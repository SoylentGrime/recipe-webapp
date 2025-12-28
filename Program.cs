using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Recipe_Webpage;
using Recipe_Webpage.Data;
using Recipe_Webpage.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Use SQL Server for production (Azure), SQLite for local development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Register the image service
builder.Services.AddScoped<IImageService, ImageService>();

// Add API Controllers
builder.Services.AddControllers();

// Add OpenAPI/Swagger for Custom GPT Actions compatibility
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Recipe Book API",
        Version = "v1",
        Description = "API for managing recipes. Compatible with OpenAI Custom GPT Actions.",
        Contact = new OpenApiContact
        {
            Name = "Recipe Book",
            Url = new Uri("https://recipe-webapp-sg2024.azurewebsites.net")
        }
    });

    // Include XML comments for better OpenAPI documentation
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Add CORS for Custom GPT Actions
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGPT", policy =>
    {
        policy.WithOrigins(
                "https://chat.openai.com",
                "https://chatgpt.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
    
    // Allow all origins for API access (can be restricted later)
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDefaultIdentity<IdentityUser>(options => 
{
    options.SignIn.RequireConfirmedAccount = false; // Simplified for demo
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages(options =>
{
    // Require Admin role for Create, Edit, Delete pages
    options.Conventions.AuthorizeFolder("/Recipes/Admin", "AdminPolicy");
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

// Seed the database with admin user and roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.InitializeAsync(services);
}

// Configure the HTTP request pipeline.

// Enable Swagger for all environments (needed for Custom GPT Actions)
app.UseSwagger(options =>
{
    options.RouteTemplate = "api/swagger/{documentName}/swagger.json";
});
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Recipe Book API v1");
    options.RoutePrefix = "api/docs";
});

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Enable CORS for API endpoints
app.UseCors("AllowAll");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.MapRazorPages();
app.MapControllers(); // Map API controllers

app.Run();
