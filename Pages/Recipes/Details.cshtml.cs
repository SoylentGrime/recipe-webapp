using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Models;
using Recipe_Webpage.Data;

namespace Recipe_Webpage.Pages.Recipes;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Recipe? Recipe { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Recipe = await _context.Recipes.FirstOrDefaultAsync(r => r.Id == id);

        if (Recipe == null)
        {
            return NotFound();
        }

        return Page();
    }
}
