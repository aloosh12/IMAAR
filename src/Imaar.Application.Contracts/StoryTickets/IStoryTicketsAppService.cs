using Imaar.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.StoryTickets
{
    public partial interface IStoryTicketsAppService : IApplicationService
    {

        Task<PagedResultDto<StoryTicketWithNavigationPropertiesDto>> GetListAsync(GetStoryTicketsInput input);

        Task<StoryTicketWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<StoryTicketDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetStoryTicketTypeLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetStoryLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<StoryTicketDto> CreateAsync(StoryTicketCreateDto input);

        Task<StoryTicketDto> UpdateAsync(Guid id, StoryTicketUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(StoryTicketExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> storyticketIds);

        Task DeleteAllAsync(GetStoryTicketsInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}