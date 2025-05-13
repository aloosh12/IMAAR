using Imaar.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.StoryLovers
{
    public partial interface IStoryLoversAppService : IApplicationService
    {

        Task<PagedResultDto<StoryLoverWithNavigationPropertiesDto>> GetListAsync(GetStoryLoversInput input);

        Task<StoryLoverWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<StoryLoverDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetStoryLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<StoryLoverDto> CreateAsync(StoryLoverCreateDto input);

        Task<StoryLoverDto> UpdateAsync(Guid id, StoryLoverUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(StoryLoverExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> storyloverIds);

        Task DeleteAllAsync(GetStoryLoversInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}