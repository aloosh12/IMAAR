using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imaar.Migrations
{
    /// <inheritdoc />
    public partial class Updated_Category_25050201292416 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TitleEn",
                table: "AppCategories");

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "AppCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "AppCategories");

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                table: "AppCategories",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
