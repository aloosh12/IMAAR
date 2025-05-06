using Imaar.UserProfiles;
using System.ComponentModel.DataAnnotations;
namespace Imaar.UserProfiles
{
    public class SecurityNumberCreateDto
    {
        //Write your custom code here...
        [Required]
        public string PhoneNumber { get; set; } = null!;
    }
}