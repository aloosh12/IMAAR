using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Imaar.MobileResponses;

namespace Imaar.Stories
{
    public partial interface IStoriesAppService
    {
        //Write your custom code here...
        [RemoteService(false)]
        Task<PagedResultDto<StoryMobileDto>> GetMobileListAsync(GetStoriesInput input);
        
        Task<MobileResponseDto> CreateWithFilesAsync(StoryCreateWithFilesDto input);
    }
}