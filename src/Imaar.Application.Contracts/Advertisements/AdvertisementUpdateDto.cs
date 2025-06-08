using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Imaar.Advertisements
{
    public abstract class AdvertisementUpdateDtoBase : IHasConcurrencyStamp
    {
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        [Required]
        public string File { get; set; } = null!;
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public Guid? UserProfileId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}