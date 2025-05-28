using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.ServiceTicketTypes
{
    public partial interface IServiceTicketTypesAppService : IApplicationService
    {

        Task<PagedResultDto<ServiceTicketTypeDto>> GetListAsync(GetServiceTicketTypesInput input);

        Task<ServiceTicketTypeDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<ServiceTicketTypeDto> CreateAsync(ServiceTicketTypeCreateDto input);

        Task<ServiceTicketTypeDto> UpdateAsync(Guid id, ServiceTicketTypeUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(ServiceTicketTypeExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> servicetickettypeIds);

        Task DeleteAllAsync(GetServiceTicketTypesInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}