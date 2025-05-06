using Imaar.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Imaar.MobileResponses
{
    public  class RegisterResponse457878
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string SecurityCode { get; set; } = null!;

        public BiologicalSex? biologicalSex = null;
        public DateOnly? dateOfBirth = null;
        public string? Password { get; set; } = null;
        public string? Latitude { get; set; } = null;
        public string? Longitude { get; set; } = null;
        public string? ProfilePhoto { get; set; } = null;

        public string? RoleId { get; set; } = null;

    }
}
