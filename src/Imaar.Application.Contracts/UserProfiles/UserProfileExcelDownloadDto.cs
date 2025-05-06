using Imaar.UserProfiles;
using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.UserProfiles
{
    public abstract class UserProfileExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public string? SecurityNumber { get; set; }
        public BiologicalSex? BiologicalSex { get; set; }
        public DateOnly? DateOfBirthMin { get; set; }
        public DateOnly? DateOfBirthMax { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }

        public UserProfileExcelDownloadDtoBase()
        {

        }
    }
}