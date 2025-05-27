using System.ComponentModel.DataAnnotations;

namespace Imaar.UserProfiles
{
    public class PasswordResetRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string SecurityCode { get; set; } = null!;

        [Required]
        public string NewPassword { get; set; } = null!;


    }
} 