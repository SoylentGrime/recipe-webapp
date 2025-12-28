using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recipe_Webpage.Data;
using Recipe_Webpage.Services;
using RecipeApp.Models;
using System.ComponentModel.DataAnnotations;

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

    public RecipesApiController(ApplicationDbContext context, IImageService imageService)
    {
        _context = context;
        _imageService = imageService;
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
                Description = r.Description,
                Ingredients = r.Ingredients,
                Instructions = r.Instructions,
                PrepTimeMinutes = r.PrepTimeMinutes,
                CookTimeMinutes = r.CookTimeMinutes,
                Servings = r.Servings,
                Category = r.Category,
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

        return Ok(new RecipeDto
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Description = recipe.Description,
            Ingredients = recipe.Ingredients,
            Instructions = recipe.Instructions,
            PrepTimeMinutes = recipe.PrepTimeMinutes,
            CookTimeMinutes = recipe.CookTimeMinutes,
            Servings = recipe.Servings,
            Category = recipe.Category,
            ImageUrl = recipe.ImageUrl,
            IsVerified = recipe.IsVerified,
            CreatedAt = recipe.CreatedAt,
            UpdatedAt = recipe.UpdatedAt
        });
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

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        var dto = new RecipeDto
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Description = recipe.Description,
            Ingredients = recipe.Ingredients,
            Instructions = recipe.Instructions,
            PrepTimeMinutes = recipe.PrepTimeMinutes,
            CookTimeMinutes = recipe.CookTimeMinutes,
            Servings = recipe.Servings,
            Category = recipe.Category,
            ImageUrl = recipe.ImageUrl,
            IsVerified = recipe.IsVerified,
            CreatedAt = recipe.CreatedAt,
            UpdatedAt = recipe.UpdatedAt
        };

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

        await _context.SaveChangesAsync();

        return Ok(new RecipeDto
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Description = recipe.Description,
            Ingredients = recipe.Ingredients,
            Instructions = recipe.Instructions,
            PrepTimeMinutes = recipe.PrepTimeMinutes,
            CookTimeMinutes = recipe.CookTimeMinutes,
            Servings = recipe.Servings,
            Category = recipe.Category,
            ImageUrl = recipe.ImageUrl,
            IsVerified = recipe.IsVerified,
            CreatedAt = recipe.CreatedAt,
            UpdatedAt = recipe.UpdatedAt
        });
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

        return Ok(new RecipeDto
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Description = recipe.Description,
            Ingredients = recipe.Ingredients,
            Instructions = recipe.Instructions,
            PrepTimeMinutes = recipe.PrepTimeMinutes,
            CookTimeMinutes = recipe.CookTimeMinutes,
            Servings = recipe.Servings,
            Category = recipe.Category,
            ImageUrl = recipe.ImageUrl,
            IsVerified = recipe.IsVerified,
            CreatedAt = recipe.CreatedAt,
            UpdatedAt = recipe.UpdatedAt
        });
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

        return Ok(new RecipeDto
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Description = recipe.Description,
            Ingredients = recipe.Ingredients,
            Instructions = recipe.Instructions,
            PrepTimeMinutes = recipe.PrepTimeMinutes,
            CookTimeMinutes = recipe.CookTimeMinutes,
            Servings = recipe.Servings,
            Category = recipe.Category,
            ImageUrl = recipe.ImageUrl,
            IsVerified = recipe.IsVerified,
            CreatedAt = recipe.CreatedAt,
            UpdatedAt = recipe.UpdatedAt
        });
    }
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

    /// <summary>Recipe title</summary>
    /// <example>Chocolate Chip Cookies</example>
    public string Title { get; set; } = string.Empty;

    /// <summary>Brief description of the recipe</summary>
    /// <example>Classic homemade chocolate chip cookies that are soft and chewy.</example>
    public string? Description { get; set; }

    /// <summary>List of ingredients, one per line</summary>
    /// <example>2 cups flour\n1 cup sugar\n1 cup butter</example>
    public string Ingredients { get; set; } = string.Empty;

    /// <summary>Step-by-step cooking instructions</summary>
    /// <example>1. Preheat oven to 375°F.\n2. Mix dry ingredients.\n3. Add wet ingredients.</example>
    public string Instructions { get; set; } = string.Empty;

    /// <summary>Preparation time in minutes</summary>
    /// <example>15</example>
    public int PrepTimeMinutes { get; set; }

    /// <summary>Cooking time in minutes</summary>
    /// <example>12</example>
    public int CookTimeMinutes { get; set; }

    /// <summary>Number of servings</summary>
    /// <example>24</example>
    public int Servings { get; set; }

    /// <summary>Recipe category</summary>
    /// <example>Cookies</example>
    public string? Category { get; set; }

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

#endregion
