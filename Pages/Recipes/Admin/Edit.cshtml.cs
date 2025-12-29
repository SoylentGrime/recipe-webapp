using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Models;
using Recipe_Webpage.Data;
using Recipe_Webpage.Services;
using System.Text.RegularExpressions;

namespace Recipe_Webpage.Pages.Recipes.Admin;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IImageService _imageService;
    private readonly ITranslationService _translationService;

    public EditModel(ApplicationDbContext context, IImageService imageService, ITranslationService translationService)
    {
        _context = context;
        _imageService = imageService;
        _translationService = translationService;
    }

    [BindProperty]
    public Recipe? Recipe { get; set; }
    
    [BindProperty]
    public IFormFile? ImageFile { get; set; }
    
    public string[] AllowedExtensions => _imageService.AllowedExtensions;
    public long MaxFileSizeMB => _imageService.MaxFileSize / (1024 * 1024);

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Recipe = await _context.Recipes.FindAsync(id);

        if (Recipe == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Recipe == null)
        {
            return NotFound();
        }

        var recipeToUpdate = await _context.Recipes.FindAsync(Recipe.Id);

        if (recipeToUpdate == null)
        {
            return NotFound();
        }

        // Handle image upload if a new file was provided
        if (ImageFile != null && ImageFile.Length > 0)
        {
            if (!_imageService.IsValidImage(ImageFile))
            {
                ModelState.AddModelError("ImageFile", $"Invalid image file. Allowed types: {string.Join(", ", AllowedExtensions)}. Max size: {MaxFileSizeMB}MB.");
                return Page();
            }
            
            var imageUrl = await _imageService.UploadImageAsync(ImageFile, Recipe.Id);
            if (imageUrl != null)
            {
                // Optionally delete the old image if it exists and is a local upload
                if (!string.IsNullOrEmpty(recipeToUpdate.ImageUrl) && 
                    recipeToUpdate.ImageUrl.StartsWith("/images/recipes/recipe-"))
                {
                    await _imageService.DeleteImageAsync(recipeToUpdate.ImageUrl);
                }
                
                recipeToUpdate.ImageUrl = imageUrl;
            }
            else
            {
                ModelState.AddModelError("ImageFile", "Failed to upload image. Please try again.");
                return Page();
            }
        }
        // If no new file uploaded, keep the existing image (don't change it)

        recipeToUpdate.Title = Recipe.Title;
        recipeToUpdate.Description = Recipe.Description;
        recipeToUpdate.Ingredients = Recipe.Ingredients;
        recipeToUpdate.Instructions = Recipe.Instructions;
        recipeToUpdate.PrepTimeMinutes = Recipe.PrepTimeMinutes;
        recipeToUpdate.CookTimeMinutes = Recipe.CookTimeMinutes;
        recipeToUpdate.Servings = Recipe.Servings;
        recipeToUpdate.Category = Recipe.Category;
        recipeToUpdate.UpdatedAt = DateTime.UtcNow;

        // Auto-translate based on input language
        await AutoTranslateRecipeAsync(recipeToUpdate);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await RecipeExistsAsync(Recipe.Id))
            {
                return NotFound();
            }
            throw;
        }

        return RedirectToPage("../Details", new { id = Recipe.Id });
    }

    private async Task AutoTranslateRecipeAsync(Recipe recipe)
    {
        if (!_translationService.IsConfigured)
        {
            return;
        }

        // Detect if the input is Chinese or English based on the title
        bool isChinese = ContainsChinese(recipe.Title);

        if (isChinese)
        {
            // Input is Chinese - store as Chinese fields and translate to English
            recipe.TitleZh = recipe.Title;
            recipe.DescriptionZh = recipe.Description;
            recipe.IngredientsZh = recipe.Ingredients;
            recipe.InstructionsZh = recipe.Instructions;
            recipe.CategoryZh = recipe.Category;

            // Translate to English
            var (titleEn, descriptionEn, ingredientsEn, instructionsEn, categoryEn) = 
                await _translationService.TranslateRecipeFieldsAsync(
                    recipe.Title, recipe.Description, recipe.Ingredients, recipe.Instructions, recipe.Category,
                    "zh", "en");

            recipe.Title = titleEn ?? recipe.Title;
            recipe.Description = descriptionEn ?? recipe.Description;
            recipe.Ingredients = ingredientsEn ?? recipe.Ingredients;
            recipe.Instructions = instructionsEn ?? recipe.Instructions;
            recipe.Category = categoryEn ?? recipe.Category;
        }
        else
        {
            // Input is English - translate to Chinese
            var (titleZh, descriptionZh, ingredientsZh, instructionsZh, categoryZh) = 
                await _translationService.TranslateRecipeFieldsAsync(
                    recipe.Title, recipe.Description, recipe.Ingredients, recipe.Instructions, recipe.Category,
                    "en", "zh");

            recipe.TitleZh = titleZh;
            recipe.DescriptionZh = descriptionZh;
            recipe.IngredientsZh = ingredientsZh;
            recipe.InstructionsZh = instructionsZh;
            recipe.CategoryZh = categoryZh;
        }
    }

    private static bool ContainsChinese(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        // Check for Chinese characters (CJK Unified Ideographs)
        return Regex.IsMatch(text, @"[\u4e00-\u9fff]");
    }

    private async Task<bool> RecipeExistsAsync(int id)
    {
        return await _context.Recipes.AnyAsync(r => r.Id == id);
    }

    public async Task<IActionResult> OnPostVerifyAsync(int id)
    {
        var recipe = await _context.Recipes.FindAsync(id);

        if (recipe == null)
        {
            return NotFound();
        }

        recipe.IsVerified = true;
        recipe.VerifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return RedirectToPage(new { id });
    }
}
