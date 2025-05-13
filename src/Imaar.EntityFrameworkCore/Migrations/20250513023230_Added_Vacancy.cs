using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imaar.Migrations
{
    /// <inheritdoc />
    public partial class Added_Vacancy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppVacancies",
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
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Longitude = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfPublish = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpectedExperience = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EducationLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkSchedule = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmploymentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BiologicalSex = table.Column<byte>(type: "tinyint", nullable: false),
                    Languages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DriveLicense = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Salary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppVacancies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppVacancies_AppServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "AppServiceTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppVacancies_AppUserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "AppUserProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppVacancies_ServiceTypeId",
                table: "AppVacancies",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppVacancies_UserProfileId",
                table: "AppVacancies",
                column: "UserProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppVacancies");
        }
    }
}
