using Imaar.UserProfiles;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.UserProfiles
{
    public abstract class UserProfileCreateDtoBase
    {
        [Required]
        public string SecurityNumber { get; set; } = null!;
        public BiologicalSex? BiologicalSex { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? ProfilePhoto { get; set; }
    }
}