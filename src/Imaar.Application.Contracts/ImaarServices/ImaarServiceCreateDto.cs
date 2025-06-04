using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.ImaarServices
{
    public abstract class ImaarServiceCreateDtoBase
    {
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public string ServiceLocation { get; set; } = null!;
        [Required]
        public string ServiceNumber { get; set; } = null!;
        public DateOnly DateOfPublish { get; set; }
        public int Price { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public int ViewCounter { get; set; } = 0;
        public int OrderCounter { get; set; } = 0;
        public Guid ServiceTypeId { get; set; }
        public Guid UserProfileId { get; set; }
    }
}