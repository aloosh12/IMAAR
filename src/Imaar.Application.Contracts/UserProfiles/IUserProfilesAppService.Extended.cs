using Imaar.MobileResponses;
using System.Threading.Tasks;

namespace Imaar.UserProfiles
{
    public partial interface IUserProfilesAppService
    {
        //Write your custom code here...
        Task<MobileResponseDto> RegisterAsync(RegisterCreateDto input);
      //  Task<MobileResponseDto> RegisterFinalAsync(RegisterCreateDt input);
    }
}