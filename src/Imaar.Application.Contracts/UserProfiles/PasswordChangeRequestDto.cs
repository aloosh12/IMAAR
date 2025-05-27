using System.ComponentModel.DataAnnotations;

namespace Imaar.UserProfiles
{
    public class PasswordChangeRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string OldPassword { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = null!;
    }
} 