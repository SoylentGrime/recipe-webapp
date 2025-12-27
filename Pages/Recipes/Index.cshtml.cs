using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Models;
using Recipe_Webpage.Data;

namespace Recipe_Webpage.Pages.Recipes;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Recipe> Recipes { get; set; } = new List<Recipe>();
    public IList<string> Categories { get; set; } = new List<string>();
    public string? SearchString { get; set; }
    public string? Category { get; set; }

    public async Task OnGetAsync(string? searchString, string? category)
    {
        SearchString = searchString;
        Category = category;

        // Get all unique categories for filter dropdown
        Categories = await _context.Recipes
            .Where(r => r.Category != null)
            .Select(r => r.Category!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        var query = _context.Recipes.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(r => 
                r.Title.Contains(searchString) || 
                (r.Description != null && r.Description.Contains(searchString)));
        }

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(r => r.Category == category);
        }

        Recipes = await query
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
}
