using Imaar.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Stories
{
    public partial interface IStoriesAppService : IApplicationService
    {

        Task<PagedResultDto<StoryWithNavigationPropertiesDto>> GetListAsync(GetStoriesInput input);

        Task<StoryWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<StoryDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<StoryDto> CreateAsync(StoryCreateDto input);

        Task<StoryDto> UpdateAsync(Guid id, StoryUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(StoryExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> storyIds);

        Task DeleteAllAsync(GetStoriesInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}