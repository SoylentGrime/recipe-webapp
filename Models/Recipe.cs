using System.ComponentModel.DataAnnotations;

namespace RecipeApp.Models;

public class Recipe
{
    public int Id { get; set; }

    // English fields
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public string Ingredients { get; set; } = string.Empty;

    [Required]
    public string Instructions { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Category { get; set; }

    // Chinese fields
    [StringLength(200)]
    public string? TitleZh { get; set; }

    [StringLength(500)]
    public string? DescriptionZh { get; set; }

    public string? IngredientsZh { get; set; }

    public string? InstructionsZh { get; set; }

    [StringLength(100)]
    public string? CategoryZh { get; set; }

    [Range(1, 1440)]
    [Display(Name = "Prep Time (minutes)")]
    public int PrepTimeMinutes { get; set; }

    [Range(1, 1440)]
    [Display(Name = "Cook Time (minutes)")]
    public int CookTimeMinutes { get; set; }

    [Range(1, 100)]
    public int Servings { get; set; } = 4;

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [Display(Name = "Verified")]
    public bool IsVerified { get; set; } = false;

    public DateTime? VerifiedAt { get; set; }

    // Helper methods to get localized content
    public string GetTitle(string culture) => culture == "zh" && !string.IsNullOrEmpty(TitleZh) ? TitleZh : Title;
    public string? GetDescription(string culture) => culture == "zh" && !string.IsNullOrEmpty(DescriptionZh) ? DescriptionZh : Description;
    public string GetIngredients(string culture) => culture == "zh" && !string.IsNullOrEmpty(IngredientsZh) ? IngredientsZh : Ingredients;
    public string GetInstructions(string culture) => culture == "zh" && !string.IsNullOrEmpty(InstructionsZh) ? InstructionsZh : Instructions;
    public string? GetCategory(string culture) => culture == "zh" && !string.IsNullOrEmpty(CategoryZh) ? CategoryZh : Category;
}
