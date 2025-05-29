using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imaar.Migrations
{
    /// <inheritdoc />
    public partial class Updated_Media_25052913270133 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppMedias_AppImaarServices_ImaarServiceId",
                table: "AppMedias");

            migrationBuilder.DropForeignKey(
                name: "FK_AppMedias_AppStories_StoryId",
                table: "AppMedias");

            migrationBuilder.DropForeignKey(
                name: "FK_AppMedias_AppVacancies_VacancyId",
                table: "AppMedias");

            migrationBuilder.DropIndex(
                name: "IX_AppMedias_ImaarServiceId",
                table: "AppMedias");

            migrationBuilder.DropIndex(
                name: "IX_AppMedias_StoryId",
                table: "AppMedias");

            migrationBuilder.DropIndex(
                name: "IX_AppMedias_VacancyId",
                table: "AppMedias");

            migrationBuilder.DropColumn(
                name: "ImaarServiceId",
                table: "AppMedias");

            migrationBuilder.DropColumn(
                name: "StoryId",
                table: "AppMedias");

            migrationBuilder.DropColumn(
                name: "VacancyId",
                table: "AppMedias");

            migrationBuilder.AddColumn<string>(
                name: "SourceEntityId",
                table: "AppMedias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte>(
                name: "SourceEntityType",
                table: "AppMedias",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "AppServiceTickets",
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
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TicketEntityType = table.Column<byte>(type: "tinyint", nullable: false),
                    TicketEntityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceTicketTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketCreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppServiceTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppServiceTickets_AppServiceTicketTypes_ServiceTicketTypeId",
                        column: x => x.ServiceTicketTypeId,
                        principalTable: "AppServiceTicketTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppServiceTickets_AppUserProfiles_TicketCreatorId",
                        column: x => x.TicketCreatorId,
                        principalTable: "AppUserProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppServiceTickets_ServiceTicketTypeId",
                table: "AppServiceTickets",
                column: "ServiceTicketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppServiceTickets_TicketCreatorId",
                table: "AppServiceTickets",
                column: "TicketCreatorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppServiceTickets");

            migrationBuilder.DropColumn(
                name: "SourceEntityId",
                table: "AppMedias");

            migrationBuilder.DropColumn(
                name: "SourceEntityType",
                table: "AppMedias");

            migrationBuilder.AddColumn<Guid>(
                name: "ImaarServiceId",
                table: "AppMedias",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoryId",
                table: "AppMedias",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VacancyId",
                table: "AppMedias",
                type: "uniqueidentifier",
                nullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_AppMedias_AppImaarServices_ImaarServiceId",
                table: "AppMedias",
                column: "ImaarServiceId",
                principalTable: "AppImaarServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AppMedias_AppStories_StoryId",
                table: "AppMedias",
                column: "StoryId",
                principalTable: "AppStories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AppMedias_AppVacancies_VacancyId",
                table: "AppMedias",
                column: "VacancyId",
                principalTable: "AppVacancies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
