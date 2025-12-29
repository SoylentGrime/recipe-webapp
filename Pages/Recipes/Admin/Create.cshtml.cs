using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Models;
using Recipe_Webpage.Data;
using Recipe_Webpage.Services;
using System.Text.RegularExpressions;

namespace Recipe_Webpage.Pages.Recipes.Admin;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IImageService _imageService;
    private readonly ITranslationService _translationService;
    private readonly ILocalizationService _localizationService;

    public CreateModel(ApplicationDbContext context, IImageService imageService, ITranslationService translationService, ILocalizationService localizationService)
    {
        _context = context;
        _imageService = imageService;
        _translationService = translationService;
        _localizationService = localizationService;
    }

    [BindProperty]
    public Recipe Recipe { get; set; } = new Recipe();
    
    [BindProperty]
    public IFormFile? ImageFile { get; set; }
    
    public string[] AllowedExtensions => _imageService.AllowedExtensions;
    public long MaxFileSizeMB => _imageService.MaxFileSize / (1024 * 1024);

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Detect input language and auto-translate
        await AutoTranslateRecipeAsync();

        // Handle image upload if a file was provided
        if (ImageFile != null && ImageFile.Length > 0)
        {
            if (!_imageService.IsValidImage(ImageFile))
            {
                ModelState.AddModelError("ImageFile", $"Invalid image file. Allowed types: {string.Join(", ", AllowedExtensions)}. Max size: {MaxFileSizeMB}MB.");
                return Page();
            }
            
            // First save the recipe to get an ID
            Recipe.CreatedAt = DateTime.UtcNow;
            _context.Recipes.Add(Recipe);
            await _context.SaveChangesAsync();
            
            // Now upload the image with the recipe ID
            var imageUrl = await _imageService.UploadImageAsync(ImageFile, Recipe.Id);
            if (imageUrl != null)
            {
                Recipe.ImageUrl = imageUrl;
                await _context.SaveChangesAsync();
            }
            
            return RedirectToPage("../Details", new { id = Recipe.Id });
        }

        Recipe.CreatedAt = DateTime.UtcNow;
        _context.Recipes.Add(Recipe);
        await _context.SaveChangesAsync();

        return RedirectToPage("../Details", new { id = Recipe.Id });
    }

    private async Task AutoTranslateRecipeAsync()
    {
        if (!_translationService.IsConfigured)
        {
            return;
        }

        // Detect if the input is Chinese or English based on the title
        bool isChinese = ContainsChinese(Recipe.Title);

        if (isChinese)
        {
            // Input is Chinese - store as Chinese fields and translate to English
            Recipe.TitleZh = Recipe.Title;
            Recipe.DescriptionZh = Recipe.Description;
            Recipe.IngredientsZh = Recipe.Ingredients;
            Recipe.InstructionsZh = Recipe.Instructions;
            Recipe.CategoryZh = Recipe.Category;

            // Translate to English
            var (titleEn, descriptionEn, ingredientsEn, instructionsEn, categoryEn) = 
                await _translationService.TranslateRecipeFieldsAsync(
                    Recipe.Title, Recipe.Description, Recipe.Ingredients, Recipe.Instructions, Recipe.Category,
                    "zh", "en");

            Recipe.Title = titleEn ?? Recipe.Title;
            Recipe.Description = descriptionEn ?? Recipe.Description;
            Recipe.Ingredients = ingredientsEn ?? Recipe.Ingredients;
            Recipe.Instructions = instructionsEn ?? Recipe.Instructions;
            Recipe.Category = categoryEn ?? Recipe.Category;
        }
        else
        {
            // Input is English - translate to Chinese
            var (titleZh, descriptionZh, ingredientsZh, instructionsZh, categoryZh) = 
                await _translationService.TranslateRecipeFieldsAsync(
                    Recipe.Title, Recipe.Description, Recipe.Ingredients, Recipe.Instructions, Recipe.Category,
                    "en", "zh");

            Recipe.TitleZh = titleZh;
            Recipe.DescriptionZh = descriptionZh;
            Recipe.IngredientsZh = ingredientsZh;
            Recipe.InstructionsZh = instructionsZh;
            Recipe.CategoryZh = categoryZh;
        }
    }

    private static bool ContainsChinese(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        // Check for Chinese characters (CJK Unified Ideographs)
        return Regex.IsMatch(text, @"[\u4e00-\u9fff]");
    }
}
