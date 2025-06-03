﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imaar.Migrations
{
    /// <inheritdoc />
    public partial class Added_StoryTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppStoryTickets",
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
                    StoryTicketTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketCreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoryAgainstId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppStoryTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppStoryTickets_AppStories_StoryAgainstId",
                        column: x => x.StoryAgainstId,
                        principalTable: "AppStories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppStoryTickets_AppStoryTicketTypes_StoryTicketTypeId",
                        column: x => x.StoryTicketTypeId,
                        principalTable: "AppStoryTicketTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppStoryTickets_AppUserProfiles_TicketCreatorId",
                        column: x => x.TicketCreatorId,
                        principalTable: "AppUserProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppStoryTickets_StoryAgainstId",
                table: "AppStoryTickets",
                column: "StoryAgainstId");

            migrationBuilder.CreateIndex(
                name: "IX_AppStoryTickets_StoryTicketTypeId",
                table: "AppStoryTickets",
                column: "StoryTicketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppStoryTickets_TicketCreatorId",
                table: "AppStoryTickets",
                column: "TicketCreatorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppStoryTickets");
        }
    }
}
