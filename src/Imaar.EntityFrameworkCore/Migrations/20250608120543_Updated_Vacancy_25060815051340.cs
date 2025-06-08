using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imaar.Migrations
{
    /// <inheritdoc />
    public partial class Updated_Vacancy_25060815051340 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppVacancyVacancyAdditionalFeature",
                columns: table => new
                {
                    VacancyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VacancyAdditionalFeatureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppVacancyVacancyAdditionalFeature", x => new { x.VacancyId, x.VacancyAdditionalFeatureId });
                    table.ForeignKey(
                        name: "FK_AppVacancyVacancyAdditionalFeature_AppVacancies_VacancyId",
                        column: x => x.VacancyId,
                        principalTable: "AppVacancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppVacancyVacancyAdditionalFeature_AppVacancyAdditionalFeatures_VacancyAdditionalFeatureId",
                        column: x => x.VacancyAdditionalFeatureId,
                        principalTable: "AppVacancyAdditionalFeatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppVacancyVacancyAdditionalFeature_VacancyAdditionalFeatureId",
                table: "AppVacancyVacancyAdditionalFeature",
                column: "VacancyAdditionalFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_AppVacancyVacancyAdditionalFeature_VacancyId_VacancyAdditionalFeatureId",
                table: "AppVacancyVacancyAdditionalFeature",
                columns: new[] { "VacancyId", "VacancyAdditionalFeatureId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppVacancyVacancyAdditionalFeature");
        }
    }
}
