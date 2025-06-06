using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imaar.Migrations
{
    /// <inheritdoc />
    public partial class Updated_Building_25060604244754 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserProfileId",
                table: "AppBuildings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AppBuildings_UserProfileId",
                table: "AppBuildings",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppBuildings_AppUserProfiles_UserProfileId",
                table: "AppBuildings",
                column: "UserProfileId",
                principalTable: "AppUserProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppBuildings_AppUserProfiles_UserProfileId",
                table: "AppBuildings");

            migrationBuilder.DropIndex(
                name: "IX_AppBuildings_UserProfileId",
                table: "AppBuildings");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "AppBuildings");
        }
    }
}
