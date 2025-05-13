using Imaar.MobileResponses;
using System.Threading.Tasks;

namespace Imaar.VerificationCodes
{
    public partial interface IVerificationCodesAppService
    {
        //Write your custom code here...
        Task<MobileResponseDto> CreateMobileAsync(string input);
        Task<bool> VerifyMobileAsync(VerificationCodeDto input);
    }
}