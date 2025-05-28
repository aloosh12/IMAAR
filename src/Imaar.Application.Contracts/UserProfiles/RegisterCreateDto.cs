using Imaar.UserProfiles;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
namespace Imaar.UserProfiles
{
    public class RegisterCreateDto
    {
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
        public string SecurityCode { get; set; } = null!;
        [Required]
        public BiologicalSex? BiologicalSex { get; set; } = null!;
        [Required]
        public DateOnly DateOfBirth { get; set; }
        [Required]
        public string? Password { get; set; } = null!;
        [Required]
        public string? Latitude { get; set; } = null!;
        [Required]
        public string? Longitude { get; set; } = null!;

        public IFormFile? ProfilePhoto { get; set; }
        [Required]
        public string? RoleId { get; set; } = null!;
    }
}