using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Imaar.Stories
{
    public partial interface IStoriesAppService
    {
        //Write your custom code here...
        [RemoteService(false)]
            Task<PagedResultDto<StoryMobileDto>> GetMobileListAsync(GetStoriesInput input);
    }
}