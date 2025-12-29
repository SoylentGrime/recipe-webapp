using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Models;
using Recipe_Webpage.Data;

namespace Recipe_Webpage.Pages.Recipes.Admin;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Recipe? Recipe { get; set; }

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
        if (Recipe == null)
        {
            return NotFound();
        }

        var recipeToDelete = await _context.Recipes.FindAsync(Recipe.Id);

        if (recipeToDelete != null)
        {
            _context.Recipes.Remove(recipeToDelete);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("../Index");
    }
}
