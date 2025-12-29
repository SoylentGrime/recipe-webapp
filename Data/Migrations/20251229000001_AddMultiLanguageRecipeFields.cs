using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recipe_Webpage.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiLanguageRecipeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TitleZh",
                table: "Recipes",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionZh",
                table: "Recipes",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IngredientsZh",
                table: "Recipes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstructionsZh",
                table: "Recipes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryZh",
                table: "Recipes",
                type: "TEXT",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TitleZh",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "DescriptionZh",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "IngredientsZh",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "InstructionsZh",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "CategoryZh",
                table: "Recipes");
        }
    }
}
