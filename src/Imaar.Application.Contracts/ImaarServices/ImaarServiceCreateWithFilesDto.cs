using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Imaar.ImaarServices
{
    public class ImaarServiceCreateWithFilesDto
    {
        [Required]
        public string Title { get; set; } = null!;
        
        [Required]
        public string Description { get; set; } = null!;
        
        [Required]
        public string ServiceLocation { get; set; } = null!;
        
        
        public int Price { get; set; }
        
        public string? Latitude { get; set; }
        
        public string? Longitude { get; set; }
        
        public Guid ServiceTypeId { get; set; }
        
        public Guid UserProfileId { get; set; }
        
        [Required]
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
    }
} 