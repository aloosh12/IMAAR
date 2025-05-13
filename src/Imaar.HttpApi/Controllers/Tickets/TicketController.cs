using Imaar.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.Tickets;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.Tickets
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Ticket")]
    [Route("api/app/tickets")]

    public abstract class TicketControllerBase : AbpController
    {
        protected ITicketsAppService _ticketsAppService;

        public TicketControllerBase(ITicketsAppService ticketsAppService)
        {
            _ticketsAppService = ticketsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<TicketWithNavigationPropertiesDto>> GetListAsync(GetTicketsInput input)
        {
            return _ticketsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<TicketWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _ticketsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<TicketDto> GetAsync(Guid id)
        {
            return _ticketsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("ticket-type-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetTicketTypeLookupAsync(LookupRequestDto input)
        {
            return _ticketsAppService.GetTicketTypeLookupAsync(input);
        }

        [HttpGet]
        [Route("user-profile-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input)
        {
            return _ticketsAppService.GetUserProfileLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<TicketDto> CreateAsync(TicketCreateDto input)
        {
            return _ticketsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<TicketDto> UpdateAsync(Guid id, TicketUpdateDto input)
        {
            return _ticketsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _ticketsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(TicketExcelDownloadDto input)
        {
            return _ticketsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _ticketsAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> ticketIds)
        {
            return _ticketsAppService.DeleteByIdsAsync(ticketIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetTicketsInput input)
        {
            return _ticketsAppService.DeleteAllAsync(input);
        }
    }
}