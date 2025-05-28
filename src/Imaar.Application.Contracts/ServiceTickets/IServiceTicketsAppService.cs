using Imaar.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.ServiceTickets
{
    public partial interface IServiceTicketsAppService : IApplicationService
    {

        Task<PagedResultDto<ServiceTicketWithNavigationPropertiesDto>> GetListAsync(GetServiceTicketsInput input);

        Task<ServiceTicketWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<ServiceTicketDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetServiceTicketTypeLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<ServiceTicketDto> CreateAsync(ServiceTicketCreateDto input);

        Task<ServiceTicketDto> UpdateAsync(Guid id, ServiceTicketUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(ServiceTicketExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> serviceticketIds);

        Task DeleteAllAsync(GetServiceTicketsInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}