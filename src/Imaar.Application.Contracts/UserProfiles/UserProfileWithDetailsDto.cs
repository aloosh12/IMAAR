using Imaar.UserProfiles;
using Imaar.UserWorksExhibitions;
using System.Collections.Generic;
using System;
namespace Imaar.UserProfiles
{
    public class UserProfileWithDetailsDto 
    {
        public string Id { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string SecurityCode { get; set; } = null!;

        public BiologicalSex? BiologicalSex = null;
        public DateOnly? DateOfBirth = null;
        public string? Latitude { get; set; } = null;
        public string? Longitude { get; set; } = null;
        public string? ProfilePhoto { get; set; } = null;

        public string? RoleId { get; set; } = null;

        public double SpeedOfCompletion { get; set; }
        public double Dealing { get; set; }
        public double Cleanliness { get; set; }
        public double Perfection { get; set; }
        public double Price { get; set; }


        public List<UserWorksExhibitionDto> UserWorksExhibitionDtos { get; set; } = null;
    }
}