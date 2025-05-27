using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.Buildings
{
    public abstract class BuildingCreateDtoBase
    {
        [Required]
        public string MainTitle { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public string Price { get; set; } = null!;
        [Required]
        public string BuildingArea { get; set; } = null!;
        [Required]
        public string NumberOfRooms { get; set; } = null!;
        [Required]
        public string NumberOfBaths { get; set; } = null!;
        [Required]
        public string FloorNo { get; set; } = null!;
        public Guid RegionId { get; set; }
        public Guid FurnishingLevelId { get; set; }
        public Guid BuildingFacadeId { get; set; }
        public Guid ServiceTypeId { get; set; }
        public List<Guid> MainAmenityIds { get; set; }
        public List<Guid> SecondaryAmenityIds { get; set; }
    }
}