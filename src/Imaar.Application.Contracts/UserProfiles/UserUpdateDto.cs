using Imaar.UserProfiles;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
namespace Imaar.UserProfiles
{
    public class UserUpdateDto
    {
        [Required]
        public string UserId { get; set; } = null!;
        //Write your custom code here...
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public BiologicalSex? BiologicalSex { get; set; } = null!;

        public IFormFile? ProfilePhoto { get; set; }
    }
}