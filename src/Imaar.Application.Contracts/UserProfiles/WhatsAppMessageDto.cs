using Imaar.UserProfiles;
using System.ComponentModel.DataAnnotations;
namespace Imaar.UserProfiles
{
    public class WhatsAppMessageDto
    {
        //Write your custom code here...
        [Required]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public string Message { get; set; } = null!;
    }
}