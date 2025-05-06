using Imaar.UserProfiles;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Imaar.UserProfiles
{
    public abstract class UserProfileUpdateDtoBase : IHasConcurrencyStamp
    {
        [Required]
        public string SecurityNumber { get; set; } = null!;
        public BiologicalSex? BiologicalSex { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? ProfilePhoto { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}