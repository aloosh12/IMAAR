using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Imaar.Buildings
{
    public class BuildingCreateWithFilesDto
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
        
        public List<Guid> MainAmenityIds { get; set; } = new List<Guid>();
        
        public List<Guid> SecondaryAmenityIds { get; set; } = new List<Guid>();
        
        [Required]
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
    }
} 