using System.ComponentModel.DataAnnotations;

namespace Imaar.UserProfiles
{
    public class VerifyEmailExistsDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
} 