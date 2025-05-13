using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imaar.Migrations
{
    /// <inheritdoc />
    public partial class Added_Media : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppMedias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    File = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ImaarServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VacancyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppMedias_AppImaarServices_ImaarServiceId",
                        column: x => x.ImaarServiceId,
                        principalTable: "AppImaarServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AppMedias_AppStories_StoryId",
                        column: x => x.StoryId,
                        principalTable: "AppStories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AppMedias_AppVacancies_VacancyId",
                        column: x => x.VacancyId,
                        principalTable: "AppVacancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppMedias_ImaarServiceId",
                table: "AppMedias",
                column: "ImaarServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMedias_StoryId",
                table: "AppMedias",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMedias_VacancyId",
                table: "AppMedias",
                column: "VacancyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppMedias");
        }
    }
}
