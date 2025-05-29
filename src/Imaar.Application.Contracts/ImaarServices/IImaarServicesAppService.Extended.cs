using System.Threading.Tasks;
using Imaar.MobileResponses;

namespace Imaar.ImaarServices
{
    public partial interface IImaarServicesAppService
    {
        //Write your custom code here...
        Task<MobileResponseDto> CreateWithFilesAsync(ImaarServiceCreateWithFilesDto input);
    }
}