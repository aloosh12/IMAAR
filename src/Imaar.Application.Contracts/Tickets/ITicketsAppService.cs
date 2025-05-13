using Imaar.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Tickets
{
    public partial interface ITicketsAppService : IApplicationService
    {

        Task<PagedResultDto<TicketWithNavigationPropertiesDto>> GetListAsync(GetTicketsInput input);

        Task<TicketWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<TicketDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetTicketTypeLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<TicketDto> CreateAsync(TicketCreateDto input);

        Task<TicketDto> UpdateAsync(Guid id, TicketUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(TicketExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> ticketIds);

        Task DeleteAllAsync(GetTicketsInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}