using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Recipe_Webpage.Data;
using RecipeApp.Models;

namespace Recipe_Webpage.Pages.Recipes;

public class PrintMultipleModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public PrintMultipleModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Recipe> Recipes { get; set; } = new();

    public async Task<IActionResult> OnGetAsync([FromQuery] string? ids, [FromQuery] bool all = false)
    {
        if (all)
        {
            // Get all recipes
            Recipes = await _context.Recipes
                .OrderBy(r => r.Title)
                .ToListAsync();
        }
        else if (!string.IsNullOrEmpty(ids))
        {
            // Parse comma-separated IDs
            var idList = ids.Split(',')
                .Select(s => int.TryParse(s.Trim(), out var id) ? id : 0)
                .Where(id => id > 0)
                .ToList();

            if (idList.Any())
            {
                Recipes = await _context.Recipes
                    .Where(r => idList.Contains(r.Id))
                    .OrderBy(r => r.Title)
                    .ToListAsync();
            }
        }

        return Page();
    }
}
