using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imaar.Migrations
{
    /// <inheritdoc />
    public partial class Added_UserFollow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUserFollows",
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
                    FollowerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FollowingUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserFollows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUserFollows_AppUserProfiles_FollowerUserId",
                        column: x => x.FollowerUserId,
                        principalTable: "AppUserProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppUserFollows_AppUserProfiles_FollowingUserId",
                        column: x => x.FollowingUserId,
                        principalTable: "AppUserProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUserFollows_FollowerUserId",
                table: "AppUserFollows",
                column: "FollowerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserFollows_FollowingUserId",
                table: "AppUserFollows",
                column: "FollowingUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUserFollows");
        }
    }
}
