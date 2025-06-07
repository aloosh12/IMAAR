using Imaar.UserProfiles;
using Volo.Abp.Application.Dtos;
using System;
using Imaar.UserProfiles;

namespace Imaar.UserProfiles
{
    public abstract class GetUserProfilesInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? SecurityNumber { get; set; }
        public BiologicalSex? BiologicalSex { get; set; }
        public DateOnly? DateOfBirthMin { get; set; }
        public DateOnly? DateOfBirthMax { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        public GetUserProfilesInputBase()
        {

        }
    }
}