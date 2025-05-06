using Imaar.UserProfiles;
using System;

namespace Imaar.UserProfiles
{
    public abstract class UserProfileExcelDtoBase
    {
        public string SecurityNumber { get; set; } = null!;
        public BiologicalSex? BiologicalSex { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? ProfilePhoto { get; set; }
    }
}