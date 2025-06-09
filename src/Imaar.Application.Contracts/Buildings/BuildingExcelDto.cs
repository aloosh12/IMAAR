using System;

namespace Imaar.Buildings
{
    public abstract class BuildingExcelDtoBase
    {
        public string MainTitle { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Price { get; set; } = null!;
        public string BuildingArea { get; set; } = null!;
        public string NumberOfRooms { get; set; } = null!;
        public string NumberOfBaths { get; set; } = null!;
        public string FloorNo { get; set; } = null!;
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public int ViewCounter { get; set; }
        public int OrderCounter { get; set; }
    }
}