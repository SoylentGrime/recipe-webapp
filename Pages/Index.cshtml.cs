using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Models;
using Recipe_Webpage.Data;

namespace Recipe_Webpage.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Recipe> RecentRecipes { get; set; } = new List<Recipe>();

    public async Task OnGetAsync()
    {
        RecentRecipes = await _context.Recipes
            .OrderByDescending(r => r.CreatedAt)
            .Take(4)
            .ToListAsync();
    }
}
