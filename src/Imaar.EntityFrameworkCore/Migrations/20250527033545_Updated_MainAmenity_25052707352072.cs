using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imaar.Migrations
{
    /// <inheritdoc />
    public partial class Updated_MainAmenity_25052707352072 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AppMainAmenities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "AppMainAmenities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AppMainAmenities");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "AppMainAmenities");
        }
    }
}
