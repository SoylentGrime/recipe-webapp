using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recipe_Webpage.Data;
using Recipe_Webpage.Services;
using RecipeApp.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Recipe_Webpage.Controllers;

/// <summary>
/// API endpoints for managing recipes. Compatible with OpenAI Custom GPT Actions.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RecipesApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IImageService _imageService;
    private readonly ITranslationService _translationService;

    public RecipesApiController(ApplicationDbContext context, IImageService imageService, ITranslationService translationService)
    {
        _context = context;
        _imageService = imageService;
        _translationService = translationService;
    }

    /// <summary>
    /// Get all recipes with optional filtering
    /// </summary>
    /// <param name="search">Search term to filter by title or description</param>
    /// <param name="category">Filter by category</param>
    /// <param name="limit">Maximum number of recipes to return (default: 50, max: 100)</param>
    /// <param name="offset">Number of recipes to skip for pagination</param>
    /// <returns>List of recipes matching the criteria</returns>
    /// <response code="200">Returns the list of recipes</response>
    [HttpGet(Name = "GetRecipes")]
    [ProducesResponseType(typeof(RecipeListResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<RecipeListResponse>> GetRecipes(
        [FromQuery] string? search = null,
        [FromQuery] string? category = null,
        [FromQuery][Range(1, 100)] int limit = 50,
        [FromQuery][Range(0, int.MaxValue)] int offset = 0)
    {
        var query = _context.Recipes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(r => 
                r.Title.ToLower().Contains(searchLower) || 
                (r.Description != null && r.Description.ToLower().Contains(searchLower)) ||
                r.Ingredients.ToLower().Contains(searchLower));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(r => r.Category != null && r.Category.ToLower() == category.ToLower());
        }

        var totalCount = await query.CountAsync();
        
        var recipes = await query
            .OrderBy(r => r.Title)
            .Skip(offset)
            .Take(limit)
            .Select(r => new RecipeDto
            {
                Id = r.Id,
                Title = r.Title,
                TitleZh = r.TitleZh,
                Description = r.Description,
                DescriptionZh = r.DescriptionZh,
                Ingredients = r.Ingredients,
                IngredientsZh = r.IngredientsZh,
                Instructions = r.Instructions,
                InstructionsZh = r.InstructionsZh,
                PrepTimeMinutes = r.PrepTimeMinutes,
                CookTimeMinutes = r.CookTimeMinutes,
                Servings = r.Servings,
                Category = r.Category,
                CategoryZh = r.CategoryZh,
                ImageUrl = r.ImageUrl,
                IsVerified = r.IsVerified,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            })
            .ToListAsync();

        return Ok(new RecipeListResponse
        {
            Recipes = recipes,
            TotalCount = totalCount,
            Limit = limit,
            Offset = offset
        });
    }

    /// <summary>
    /// Get a specific recipe by ID
    /// </summary>
    /// <param name="id">The recipe ID</param>
    /// <returns>The recipe details</returns>
    /// <response code="200">Returns the recipe</response>
    /// <response code="404">Recipe not found</response>
    [HttpGet("{id}", Name = "GetRecipeById")]
    [ProducesResponseType(typeof(RecipeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeDto>> GetRecipe(int id)
    {
        var recipe = await _context.Recipes.FindAsync(id);

        if (recipe == null)
        {
            return NotFound(new ErrorResponse { Error = "Recipe not found", Message = $"No recipe exists with ID {id}" });
        }

        return Ok(ToRecipeDto(recipe));
    }

    /// <summary>
    /// Get all available recipe categories
    /// </summary>
    /// <returns>List of unique categories</returns>
    /// <response code="200">Returns the list of categories</response>
    [HttpGet("categories", Name = "GetCategories")]
    [ProducesResponseType(typeof(CategoriesResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CategoriesResponse>> GetCategories()
    {
        var categories = await _context.Recipes
            .Where(r => r.Category != null)
            .Select(r => r.Category!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        return Ok(new CategoriesResponse { Categories = categories });
    }

    /// <summary>
    /// Create a new recipe
    /// </summary>
    /// <param name="request">The recipe data to create</param>
    /// <returns>The created recipe</returns>
    /// <response code="201">Recipe created successfully</response>
    /// <response code="400">Invalid recipe data</response>
    [HttpPost(Name = "CreateRecipe")]
    [ProducesResponseType(typeof(RecipeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecipeDto>> CreateRecipe([FromBody] CreateRecipeRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse 
            { 
                Error = "Validation failed", 
                Message = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
            });
        }

        var recipe = new Recipe
        {
            Title = request.Title,
            Description = request.Description,
            Ingredients = request.Ingredients,
            Instructions = request.Instructions,
            PrepTimeMinutes = request.PrepTimeMinutes,
            CookTimeMinutes = request.CookTimeMinutes,
            Servings = request.Servings,
            Category = request.Category,
            CreatedAt = DateTime.UtcNow,
            IsVerified = false
        };

        // Auto-translate based on input language
        await AutoTranslateRecipeAsync(recipe);

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        var dto = ToRecipeDto(recipe);

        return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, dto);
    }

    /// <summary>
    /// Update an existing recipe
    /// </summary>
    /// <param name="id">The recipe ID to update</param>
    /// <param name="request">The updated recipe data</param>
    /// <returns>The updated recipe</returns>
    /// <response code="200">Recipe updated successfully</response>
    /// <response code="400">Invalid recipe data</response>
    /// <response code="404">Recipe not found</response>
    [HttpPut("{id}", Name = "UpdateRecipe")]
    [ProducesResponseType(typeof(RecipeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeDto>> UpdateRecipe(int id, [FromBody] UpdateRecipeRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse 
            { 
                Error = "Validation failed", 
                Message = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
            });
        }

        var recipe = await _context.Recipes.FindAsync(id);

        if (recipe == null)
        {
            return NotFound(new ErrorResponse { Error = "Recipe not found", Message = $"No recipe exists with ID {id}" });
        }

        // Update only provided fields
        if (request.Title != null) recipe.Title = request.Title;
        if (request.Description != null) recipe.Description = request.Description;
        if (request.Ingredients != null) recipe.Ingredients = request.Ingredients;
        if (request.Instructions != null) recipe.Instructions = request.Instructions;
        if (request.PrepTimeMinutes.HasValue) recipe.PrepTimeMinutes = request.PrepTimeMinutes.Value;
        if (request.CookTimeMinutes.HasValue) recipe.CookTimeMinutes = request.CookTimeMinutes.Value;
        if (request.Servings.HasValue) recipe.Servings = request.Servings.Value;
        if (request.Category != null) recipe.Category = request.Category;
        
        recipe.UpdatedAt = DateTime.UtcNow;

        // Auto-translate based on input language
        await AutoTranslateRecipeAsync(recipe);

        await _context.SaveChangesAsync();

        return Ok(ToRecipeDto(recipe));
    }

    /// <summary>
    /// Upload an image for a recipe
    /// </summary>
    /// <param name="id">The recipe ID</param>
    /// <param name="file">The image file to upload (jpg, jpeg, png, gif, webp - max 5MB)</param>
    /// <returns>The updated recipe with image URL</returns>
    /// <response code="200">Image uploaded successfully</response>
    /// <response code="400">Invalid image file</response>
    /// <response code="404">Recipe not found</response>
    [HttpPost("{id}/image", Name = "UploadRecipeImage")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(RecipeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeDto>> UploadRecipeImage(int id, IFormFile file)
    {
        var recipe = await _context.Recipes.FindAsync(id);

        if (recipe == null)
        {
            return NotFound(new ErrorResponse { Error = "Recipe not found", Message = $"No recipe exists with ID {id}" });
        }

        if (file == null || file.Length == 0)
        {
            return BadRequest(new ErrorResponse { Error = "No file provided", Message = "Please provide an image file to upload" });
        }

        if (!_imageService.IsValidImage(file))
        {
            return BadRequest(new ErrorResponse 
            { 
                Error = "Invalid image", 
                Message = $"Invalid image file. Allowed types: {string.Join(", ", _imageService.AllowedExtensions)}. Max size: {_imageService.MaxFileSize / (1024 * 1024)}MB" 
            });
        }

        // Delete old image if exists
        if (!string.IsNullOrEmpty(recipe.ImageUrl) && recipe.ImageUrl.StartsWith("/images/recipes/"))
        {
            await _imageService.DeleteImageAsync(recipe.ImageUrl);
        }

        var imageUrl = await _imageService.UploadImageAsync(file, recipe.Id);
        if (imageUrl == null)
        {
            return BadRequest(new ErrorResponse { Error = "Upload failed", Message = "Failed to upload image. Please try again." });
        }

        recipe.ImageUrl = imageUrl;
        recipe.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(ToRecipeDto(recipe));
    }

    /// <summary>
    /// Delete the image for a recipe
    /// </summary>
    /// <param name="id">The recipe ID</param>
    /// <returns>The updated recipe without image</returns>
    /// <response code="200">Image deleted successfully</response>
    /// <response code="404">Recipe not found</response>
    [HttpDelete("{id}/image", Name = "DeleteRecipeImage")]
    [ProducesResponseType(typeof(RecipeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeDto>> DeleteRecipeImage(int id)
    {
        var recipe = await _context.Recipes.FindAsync(id);

        if (recipe == null)
        {
            return NotFound(new ErrorResponse { Error = "Recipe not found", Message = $"No recipe exists with ID {id}" });
        }

        // Delete old image if exists
        if (!string.IsNullOrEmpty(recipe.ImageUrl) && recipe.ImageUrl.StartsWith("/images/recipes/"))
        {
            await _imageService.DeleteImageAsync(recipe.ImageUrl);
        }

        recipe.ImageUrl = null;
        recipe.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(ToRecipeDto(recipe));
    }

    /// <summary>
    /// Upload an image for a recipe using base64 encoded data (for ChatGPT/API clients)
    /// </summary>
    /// <param name="id">The recipe ID</param>
    /// <param name="request">The base64 encoded image data</param>
    /// <returns>The updated recipe with image URL</returns>
    /// <response code="200">Image uploaded successfully</response>
    /// <response code="400">Invalid image data</response>
    /// <response code="404">Recipe not found</response>
    [HttpPut("{id}/image", Name = "UploadRecipeImageBase64")]
    [ProducesResponseType(typeof(RecipeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeDto>> UploadRecipeImageBase64(int id, [FromBody] UploadImageBase64Request request)
    {
        var recipe = await _context.Recipes.FindAsync(id);

        if (recipe == null)
        {
            return NotFound(new ErrorResponse { Error = "Recipe not found", Message = $"No recipe exists with ID {id}" });
        }

        if (string.IsNullOrWhiteSpace(request.ImageBase64))
        {
            return BadRequest(new ErrorResponse { Error = "No image data", Message = "Please provide base64 encoded image data" });
        }

        // Validate and parse the base64 data
        byte[] imageBytes;
        string extension;
        try
        {
            // Handle data URL format: data:image/jpeg;base64,/9j/4AAQ...
            var base64Data = request.ImageBase64;
            if (base64Data.Contains(","))
            {
                var parts = base64Data.Split(',');
                var header = parts[0].ToLower();
                base64Data = parts[1];
                
                // Determine extension from MIME type
                if (header.Contains("image/jpeg") || header.Contains("image/jpg"))
                    extension = ".jpg";
                else if (header.Contains("image/png"))
                    extension = ".png";
                else if (header.Contains("image/gif"))
                    extension = ".gif";
                else if (header.Contains("image/webp"))
                    extension = ".webp";
                else
                    return BadRequest(new ErrorResponse { Error = "Invalid image type", Message = "Allowed types: jpg, jpeg, png, gif, webp" });
            }
            else
            {
                // Default to jpg if no header
                extension = !string.IsNullOrWhiteSpace(request.FileName) 
                    ? Path.GetExtension(request.FileName).ToLowerInvariant() 
                    : ".jpg";
                
                if (!new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }.Contains(extension))
                    extension = ".jpg";
            }

            imageBytes = Convert.FromBase64String(base64Data);
        }
        catch (FormatException)
        {
            return BadRequest(new ErrorResponse { Error = "Invalid base64", Message = "The provided data is not valid base64" });
        }

        // Check file size (5MB max)
        if (imageBytes.Length > 5 * 1024 * 1024)
        {
            return BadRequest(new ErrorResponse { Error = "File too large", Message = "Maximum file size is 5MB" });
        }

        // Save the image
        try
        {
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "recipes");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var uniqueId = Guid.NewGuid().ToString("N")[..8];
            var fileName = $"recipe-{id}-{timestamp}-{uniqueId}{extension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

            // Delete old image if exists
            if (!string.IsNullOrEmpty(recipe.ImageUrl) && recipe.ImageUrl.StartsWith("/images/recipes/"))
            {
                await _imageService.DeleteImageAsync(recipe.ImageUrl);
            }

            recipe.ImageUrl = $"/images/recipes/{fileName}";
            recipe.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(ToRecipeDto(recipe));
        }
        catch (Exception)
        {
            return BadRequest(new ErrorResponse { Error = "Upload failed", Message = "Failed to save image. Please try again." });
        }
    }

    #region Helper Methods

    private static RecipeDto ToRecipeDto(Recipe recipe) => new()
    {
        Id = recipe.Id,
        Title = recipe.Title,
        TitleZh = recipe.TitleZh,
        Description = recipe.Description,
        DescriptionZh = recipe.DescriptionZh,
        Ingredients = recipe.Ingredients,
        IngredientsZh = recipe.IngredientsZh,
        Instructions = recipe.Instructions,
        InstructionsZh = recipe.InstructionsZh,
        PrepTimeMinutes = recipe.PrepTimeMinutes,
        CookTimeMinutes = recipe.CookTimeMinutes,
        Servings = recipe.Servings,
        Category = recipe.Category,
        CategoryZh = recipe.CategoryZh,
        ImageUrl = recipe.ImageUrl,
        IsVerified = recipe.IsVerified,
        CreatedAt = recipe.CreatedAt,
        UpdatedAt = recipe.UpdatedAt
    };

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

    #endregion
}

#region DTOs for OpenAPI Schema

/// <summary>
/// Recipe data transfer object
/// </summary>
public class RecipeDto
{
    /// <summary>Unique identifier for the recipe</summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>Recipe title (English)</summary>
    /// <example>Chocolate Chip Cookies</example>
    public string Title { get; set; } = string.Empty;

    /// <summary>Recipe title (Chinese)</summary>
    /// <example>巧克力曲奇饼干</example>
    public string? TitleZh { get; set; }

    /// <summary>Brief description of the recipe (English)</summary>
    /// <example>Classic homemade chocolate chip cookies that are soft and chewy.</example>
    public string? Description { get; set; }

    /// <summary>Brief description of the recipe (Chinese)</summary>
    /// <example>经典自制巧克力曲奇饼干，柔软耐嚼。</example>
    public string? DescriptionZh { get; set; }

    /// <summary>List of ingredients, one per line (English)</summary>
    /// <example>2 cups flour\n1 cup sugar\n1 cup butter</example>
    public string Ingredients { get; set; } = string.Empty;

    /// <summary>List of ingredients, one per line (Chinese)</summary>
    /// <example>2杯面粉\n1杯糖\n1杯黄油</example>
    public string? IngredientsZh { get; set; }

    /// <summary>Step-by-step cooking instructions (English)</summary>
    /// <example>1. Preheat oven to 375°F.\n2. Mix dry ingredients.\n3. Add wet ingredients.</example>
    public string Instructions { get; set; } = string.Empty;

    /// <summary>Step-by-step cooking instructions (Chinese)</summary>
    /// <example>1. 预热烤箱至190°C。\n2. 混合干成分。\n3. 加入湿成分。</example>
    public string? InstructionsZh { get; set; }

    /// <summary>Preparation time in minutes</summary>
    /// <example>15</example>
    public int PrepTimeMinutes { get; set; }

    /// <summary>Cooking time in minutes</summary>
    /// <example>12</example>
    public int CookTimeMinutes { get; set; }

    /// <summary>Number of servings</summary>
    /// <example>24</example>
    public int Servings { get; set; }

    /// <summary>Recipe category (English)</summary>
    /// <example>Cookies</example>
    public string? Category { get; set; }

    /// <summary>Recipe category (Chinese)</summary>
    /// <example>饼干</example>
    public string? CategoryZh { get; set; }

    /// <summary>URL to recipe image</summary>
    /// <example>/images/recipes/chocolate-chip-cookies.jpg</example>
    public string? ImageUrl { get; set; }

    /// <summary>Whether the recipe has been verified</summary>
    /// <example>true</example>
    public bool IsVerified { get; set; }

    /// <summary>When the recipe was created</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>When the recipe was last updated</summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Response containing a list of recipes
/// </summary>
public class RecipeListResponse
{
    /// <summary>List of recipes</summary>
    public List<RecipeDto> Recipes { get; set; } = new();

    /// <summary>Total number of recipes matching the query</summary>
    /// <example>94</example>
    public int TotalCount { get; set; }

    /// <summary>Maximum number of recipes returned</summary>
    /// <example>50</example>
    public int Limit { get; set; }

    /// <summary>Number of recipes skipped</summary>
    /// <example>0</example>
    public int Offset { get; set; }
}

/// <summary>
/// Response containing available categories
/// </summary>
public class CategoriesResponse
{
    /// <summary>List of unique recipe categories</summary>
    public List<string> Categories { get; set; } = new();
}

/// <summary>
/// Request to create a new recipe
/// </summary>
public class CreateRecipeRequest
{
    /// <summary>Recipe title (required)</summary>
    /// <example>Chocolate Chip Cookies</example>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    /// <summary>Brief description of the recipe</summary>
    /// <example>Classic homemade chocolate chip cookies that are soft and chewy.</example>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>List of ingredients, one per line (required)</summary>
    /// <example>2 cups flour\n1 cup sugar\n1 cup butter</example>
    [Required]
    public string Ingredients { get; set; } = string.Empty;

    /// <summary>Step-by-step cooking instructions (required)</summary>
    /// <example>1. Preheat oven to 375°F.\n2. Mix dry ingredients.\n3. Add wet ingredients.</example>
    [Required]
    public string Instructions { get; set; } = string.Empty;

    /// <summary>Preparation time in minutes</summary>
    /// <example>15</example>
    [Range(0, 1440)]
    public int PrepTimeMinutes { get; set; }

    /// <summary>Cooking time in minutes</summary>
    /// <example>12</example>
    [Range(0, 1440)]
    public int CookTimeMinutes { get; set; }

    /// <summary>Number of servings</summary>
    /// <example>24</example>
    [Range(1, 1000)]
    public int Servings { get; set; } = 4;

    /// <summary>Recipe category</summary>
    /// <example>Cookies</example>
    [StringLength(100)]
    public string? Category { get; set; }
}

/// <summary>
/// Request to update an existing recipe. All fields are optional - only provided fields will be updated.
/// </summary>
public class UpdateRecipeRequest
{
    /// <summary>Recipe title</summary>
    /// <example>Chocolate Chip Cookies</example>
    [StringLength(200, MinimumLength = 1)]
    public string? Title { get; set; }

    /// <summary>Brief description of the recipe</summary>
    /// <example>Classic homemade chocolate chip cookies that are soft and chewy.</example>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>List of ingredients, one per line</summary>
    /// <example>2 cups flour\n1 cup sugar\n1 cup butter</example>
    public string? Ingredients { get; set; }

    /// <summary>Step-by-step cooking instructions</summary>
    /// <example>1. Preheat oven to 375°F.\n2. Mix dry ingredients.\n3. Add wet ingredients.</example>
    public string? Instructions { get; set; }

    /// <summary>Preparation time in minutes</summary>
    /// <example>15</example>
    [Range(0, 1440)]
    public int? PrepTimeMinutes { get; set; }

    /// <summary>Cooking time in minutes</summary>
    /// <example>12</example>
    [Range(0, 1440)]
    public int? CookTimeMinutes { get; set; }

    /// <summary>Number of servings</summary>
    /// <example>24</example>
    [Range(1, 1000)]
    public int? Servings { get; set; }

    /// <summary>Recipe category</summary>
    /// <example>Cookies</example>
    [StringLength(100)]
    public string? Category { get; set; }
}

/// <summary>
/// Error response
/// </summary>
public class ErrorResponse
{
    /// <summary>Error type</summary>
    /// <example>Recipe not found</example>
    public string Error { get; set; } = string.Empty;

    /// <summary>Detailed error message</summary>
    /// <example>No recipe exists with ID 999</example>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Request to upload an image using base64 encoding
/// </summary>
public class UploadImageBase64Request
{
    /// <summary>Base64 encoded image data. Can include data URL prefix (e.g., data:image/jpeg;base64,...) or just the base64 string</summary>
    /// <example>data:image/jpeg;base64,/9j/4AAQSkZJRg...</example>
    [Required]
    public string ImageBase64 { get; set; } = string.Empty;

    /// <summary>Optional filename with extension to determine image type (e.g., recipe.jpg)</summary>
    /// <example>my-recipe-photo.jpg</example>
    public string? FileName { get; set; }
}

#endregion
