using System.Threading.Tasks;
using Imaar.MobileResponses;
using Volo.Abp.Application.Dtos;

namespace Imaar.ImaarServices
{
    public partial interface IImaarServicesAppService
    {
        //Write your custom code here...
        Task<MobileResponseDto> CreateWithFilesAsync(ImaarServiceCreateWithFilesDto input);
        
        Task<PagedResultDto<ImaarServiceShopListItemDto>> GetShopListAsync(ImaarServiceFilterDto input);
    }
}