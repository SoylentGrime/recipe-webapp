using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Models;
using Recipe_Webpage.Data;
using Recipe_Webpage.Services;

namespace Recipe_Webpage.Pages.Recipes.Admin;

[Authorize]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IImageService _imageService;

    public EditModel(ApplicationDbContext context, IImageService imageService)
    {
        _context = context;
        _imageService = imageService;
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
