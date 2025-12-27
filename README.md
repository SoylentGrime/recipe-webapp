# Recipe Book - ASP.NET Core Web Application

A full-featured recipe management web application built with ASP.NET Core 10 and Entity Framework Core, designed for easy deployment to Azure.

## Features

- **Browse Recipes**: View all recipes with search and category filtering
- **Recipe Details**: Full recipe view with ingredients, instructions, prep/cook times
- **Admin Management**: Create, edit, and delete recipes (requires admin login)
- **Print Friendly**: Clean print layout for any recipe
- **Azure Ready**: Configured for Azure App Service and Azure SQL Database

## Technology Stack

- **.NET 10** - Latest .NET framework
- **ASP.NET Core Razor Pages** - Server-side web UI
- **Entity Framework Core 10** - ORM with SQL Server/SQLite support
- **ASP.NET Core Identity** - Authentication and authorization
- **Bootstrap 5** - Responsive UI framework
- **Bootstrap Icons** - Icon library

## Getting Started

### Prerequisites

- .NET 10 SDK
- SQL Server (for production) or SQLite (for development)

### Local Development

1. Clone the repository
2. Navigate to the project directory
3. Run the application:

```bash
dotnet run
```

4. Open https://localhost:5001 in your browser

### Default Admin Credentials

- **Email**: admin@recipes.local
- **Password**: Admin123!

> ⚠️ **Important**: Change these credentials in production!

## Azure Deployment

### Quick Deploy with Azure CLI

1. **Create Azure Resources**:

```bash
# Login to Azure
az login

# Create resource group
az group create --name RecipeApp-RG --location eastus

# Create App Service Plan
az appservice plan create --name RecipeAppPlan --resource-group RecipeApp-RG --sku B1 --is-linux

# Create Web App
az webapp create --name your-recipe-app --resource-group RecipeApp-RG --plan RecipeAppPlan --runtime "DOTNET|10.0"

# Create SQL Server
az sql server create --name your-sql-server --resource-group RecipeApp-RG --location eastus --admin-user sqladmin --admin-password YourStrongPassword123!

# Create SQL Database
az sql db create --resource-group RecipeApp-RG --server your-sql-server --name RecipeDb --service-objective S0

# Allow Azure services to access SQL Server
az sql server firewall-rule create --resource-group RecipeApp-RG --server your-sql-server --name AllowAzure --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0
```

2. **Configure Connection String**:

```bash
# Set connection string in App Service
az webapp config connection-string set --name your-recipe-app --resource-group RecipeApp-RG --settings DefaultConnection="Server=tcp:your-sql-server.database.windows.net,1433;Initial Catalog=RecipeDb;Persist Security Info=False;User ID=sqladmin;Password=YourStrongPassword123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" --connection-string-type SQLAzure
```

3. **Deploy the Application**:

```bash
# Publish the application
dotnet publish -c Release -o ./publish

# Deploy using ZIP deploy
cd publish
zip -r ../deploy.zip .
cd ..
az webapp deployment source config-zip --name your-recipe-app --resource-group RecipeApp-RG --src deploy.zip
```

### Deploy via Visual Studio / VS Code

1. Right-click project → Publish
2. Select Azure App Service
3. Follow the wizard to create/select resources
4. Configure connection string in Azure Portal

### GitHub Actions CI/CD

Create `.github/workflows/azure-deploy.yml` for automated deployments.

## Configuration

### appsettings.json

Local development uses SQLite by default. For production:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:YOUR-SERVER.database.windows.net,1433;Initial Catalog=RecipeDb;..."
  }
}
```

### Environment Variables (Azure)

Set these in Azure App Service Configuration:
- `ASPNETCORE_ENVIRONMENT`: Production
- Connection string via Connection Strings section

## Project Structure

```
Recipe Webpage/
├── Data/
│   ├── ApplicationDbContext.cs    # EF Core DbContext
│   └── SeedData.cs                # Database seeding
├── Models/
│   └── Recipe.cs                  # Recipe entity
├── Pages/
│   ├── Index.cshtml               # Home page
│   ├── Recipes/
│   │   ├── Index.cshtml           # Recipe list
│   │   ├── Details.cshtml         # Recipe details
│   │   └── Admin/
│   │       ├── Create.cshtml      # Create recipe (admin)
│   │       ├── Edit.cshtml        # Edit recipe (admin)
│   │       └── Delete.cshtml      # Delete recipe (admin)
│   └── Shared/
│       └── _Layout.cshtml         # Main layout
├── wwwroot/                       # Static files
├── Program.cs                     # Application entry point
├── appsettings.json              # Configuration
└── appsettings.Production.json   # Production config
```

## Security Notes

1. **Change default admin password** immediately in production
2. Use **Azure Managed Identity** for database connections when possible
3. Enable **HTTPS only** in Azure App Service
4. Configure **CORS** if needed for API access
5. Use **Key Vault** for sensitive configuration values

## License

MIT License
