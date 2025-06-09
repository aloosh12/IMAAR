using System;
using System.Threading.Tasks;
using Imaar.MobileResponses;
using Volo.Abp.Application.Dtos;

namespace Imaar.ImaarServices
{
    public partial interface IImaarServicesAppService
    {
        //Write your custom code here...
        Task<MobileResponseDto> CreateWithFilesAsync(ImaarServiceCreateWithFilesDto input);
        
        Task<MobileResponseDto> GetImaarServiceWithDetailsAsync(Guid id);
        
        Task<PagedResultDto<ImaarServiceShopListItemDto>> GetShopListAsync(GetShopListInput input);
        
        Task<MobileResponseDto> IncrementViewCounterAsync(Guid id);
        
        Task<MobileResponseDto> IncrementOrderCounterAsync(Guid id);
    }
}