using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.StoryTicketTypes
{
    public partial interface IStoryTicketTypesAppService : IApplicationService
    {

        Task<PagedResultDto<StoryTicketTypeDto>> GetListAsync(GetStoryTicketTypesInput input);

        Task<StoryTicketTypeDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<StoryTicketTypeDto> CreateAsync(StoryTicketTypeCreateDto input);

        Task<StoryTicketTypeDto> UpdateAsync(Guid id, StoryTicketTypeUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(StoryTicketTypeExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> storytickettypeIds);

        Task DeleteAllAsync(GetStoryTicketTypesInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}