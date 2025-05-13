using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imaar.Migrations
{
    /// <inheritdoc />
    public partial class Added_Ticket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppTickets",
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
                    TicketTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketCreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketAgainstId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppTickets_AppTicketTypes_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalTable: "AppTicketTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppTickets_AppUserProfiles_TicketAgainstId",
                        column: x => x.TicketAgainstId,
                        principalTable: "AppUserProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppTickets_AppUserProfiles_TicketCreatorId",
                        column: x => x.TicketCreatorId,
                        principalTable: "AppUserProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppTickets_TicketAgainstId",
                table: "AppTickets",
                column: "TicketAgainstId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTickets_TicketCreatorId",
                table: "AppTickets",
                column: "TicketCreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTickets_TicketTypeId",
                table: "AppTickets",
                column: "TicketTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppTickets");
        }
    }
}
