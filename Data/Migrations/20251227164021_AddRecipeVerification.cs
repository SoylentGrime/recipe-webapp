using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recipe_Webpage.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Recipes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "Recipes",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "Recipes");
        }
    }
}
