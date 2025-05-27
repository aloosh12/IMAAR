using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imaar.Migrations
{
    /// <inheritdoc />
    public partial class Added_Building : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppBuildings",
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
                    MainTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BuildingArea = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfRooms = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfBaths = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FloorNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FurnishingLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuildingFacadeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppBuildings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppBuildings_AppBuildingFacades_BuildingFacadeId",
                        column: x => x.BuildingFacadeId,
                        principalTable: "AppBuildingFacades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppBuildings_AppFurnishingLevels_FurnishingLevelId",
                        column: x => x.FurnishingLevelId,
                        principalTable: "AppFurnishingLevels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppBuildings_AppRegions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "AppRegions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppBuildings_AppServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "AppServiceTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AppBuildingMainAmenity",
                columns: table => new
                {
                    BuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MainAmenityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppBuildingMainAmenity", x => new { x.BuildingId, x.MainAmenityId });
                    table.ForeignKey(
                        name: "FK_AppBuildingMainAmenity_AppBuildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "AppBuildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppBuildingMainAmenity_AppMainAmenities_MainAmenityId",
                        column: x => x.MainAmenityId,
                        principalTable: "AppMainAmenities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppBuildingSecondaryAmenity",
                columns: table => new
                {
                    BuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SecondaryAmenityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppBuildingSecondaryAmenity", x => new { x.BuildingId, x.SecondaryAmenityId });
                    table.ForeignKey(
                        name: "FK_AppBuildingSecondaryAmenity_AppBuildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "AppBuildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppBuildingSecondaryAmenity_AppSecondaryAmenities_SecondaryAmenityId",
                        column: x => x.SecondaryAmenityId,
                        principalTable: "AppSecondaryAmenities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppBuildingMainAmenity_BuildingId_MainAmenityId",
                table: "AppBuildingMainAmenity",
                columns: new[] { "BuildingId", "MainAmenityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppBuildingMainAmenity_MainAmenityId",
                table: "AppBuildingMainAmenity",
                column: "MainAmenityId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBuildings_BuildingFacadeId",
                table: "AppBuildings",
                column: "BuildingFacadeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBuildings_FurnishingLevelId",
                table: "AppBuildings",
                column: "FurnishingLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBuildings_RegionId",
                table: "AppBuildings",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBuildings_ServiceTypeId",
                table: "AppBuildings",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBuildingSecondaryAmenity_BuildingId_SecondaryAmenityId",
                table: "AppBuildingSecondaryAmenity",
                columns: new[] { "BuildingId", "SecondaryAmenityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppBuildingSecondaryAmenity_SecondaryAmenityId",
                table: "AppBuildingSecondaryAmenity",
                column: "SecondaryAmenityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppBuildingMainAmenity");

            migrationBuilder.DropTable(
                name: "AppBuildingSecondaryAmenity");

            migrationBuilder.DropTable(
                name: "AppBuildings");
        }
    }
}
