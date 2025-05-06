using Imaar.UserProfiles;
using System.ComponentModel.DataAnnotations;
namespace Imaar.UserProfiles
{
    public class SecurityNumberDto
    {
        //Write your custom code here...
        [Required]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        public string SecurityCode { get; set; } = null!;
    }
}