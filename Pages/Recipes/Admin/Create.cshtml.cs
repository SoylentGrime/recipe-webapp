using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Models;
using Recipe_Webpage.Data;
using Recipe_Webpage.Services;

namespace Recipe_Webpage.Pages.Recipes.Admin;

[Authorize]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IImageService _imageService;

    public CreateModel(ApplicationDbContext context, IImageService imageService)
    {
        _context = context;
        _imageService = imageService;
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
}
