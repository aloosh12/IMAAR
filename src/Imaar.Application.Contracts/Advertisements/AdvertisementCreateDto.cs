using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.Advertisements
{
    public abstract class AdvertisementCreateDtoBase
    {
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        [Required]
        public string File { get; set; } = null!;
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        public int Order { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public Guid? UserProfileId { get; set; }
    }
}