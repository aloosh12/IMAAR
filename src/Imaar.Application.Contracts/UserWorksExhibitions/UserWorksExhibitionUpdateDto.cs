using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Imaar.UserWorksExhibitions
{
    public abstract class UserWorksExhibitionUpdateDtoBase : IHasConcurrencyStamp
    {
        public string? Title { get; set; }
        [Required]
        public string File { get; set; } = null!;
        public int Order { get; set; }
        public Guid UserProfileId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}