using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.TicketTypes
{
    public partial interface ITicketTypesAppService : IApplicationService
    {

        Task<PagedResultDto<TicketTypeDto>> GetListAsync(GetTicketTypesInput input);

        Task<TicketTypeDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<TicketTypeDto> CreateAsync(TicketTypeCreateDto input);

        Task<TicketTypeDto> UpdateAsync(Guid id, TicketTypeUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(TicketTypeExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> tickettypeIds);

        Task DeleteAllAsync(GetTicketTypesInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}