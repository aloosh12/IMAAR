using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imaar.Migrations
{
    /// <inheritdoc />
    public partial class Added_Evalauation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppEvalauations",
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
                    SpeedOfCompletion = table.Column<int>(type: "int", nullable: false),
                    Dealing = table.Column<int>(type: "int", nullable: false),
                    Cleanliness = table.Column<int>(type: "int", nullable: false),
                    Perfection = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    Evaluatord = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvaluatedPersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppEvalauations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppEvalauations_AppUserProfiles_EvaluatedPersonId",
                        column: x => x.EvaluatedPersonId,
                        principalTable: "AppUserProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppEvalauations_AppUserProfiles_Evaluatord",
                        column: x => x.Evaluatord,
                        principalTable: "AppUserProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppEvalauations_EvaluatedPersonId",
                table: "AppEvalauations",
                column: "EvaluatedPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_AppEvalauations_Evaluatord",
                table: "AppEvalauations",
                column: "Evaluatord");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppEvalauations");
        }
    }
}
