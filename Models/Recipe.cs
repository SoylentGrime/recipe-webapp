using System.ComponentModel.DataAnnotations;

namespace RecipeApp.Models;

public class Recipe
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public string Ingredients { get; set; } = string.Empty;

    [Required]
    public string Instructions { get; set; } = string.Empty;

    [Range(1, 1440)]
    [Display(Name = "Prep Time (minutes)")]
    public int PrepTimeMinutes { get; set; }

    [Range(1, 1440)]
    [Display(Name = "Cook Time (minutes)")]
    public int CookTimeMinutes { get; set; }

    [Range(1, 100)]
    public int Servings { get; set; } = 4;

    [StringLength(100)]
    public string? Category { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
