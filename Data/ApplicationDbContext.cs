using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Models;

namespace Recipe_Webpage.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Recipe> Recipes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Recipe>(entity =>
        {
            entity.HasIndex(r => r.Title);
            entity.HasIndex(r => r.Category);
            entity.HasIndex(r => r.CreatedAt);
        });
    }
}
