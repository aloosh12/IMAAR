using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.TicketTypes;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.TicketTypes
{
    [RemoteService]
    [Area("app")]
    [ControllerName("TicketType")]
    [Route("api/app/ticket-types")]

    public abstract class TicketTypeControllerBase : AbpController
    {
        protected ITicketTypesAppService _ticketTypesAppService;

        public TicketTypeControllerBase(ITicketTypesAppService ticketTypesAppService)
        {
            _ticketTypesAppService = ticketTypesAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<TicketTypeDto>> GetListAsync(GetTicketTypesInput input)
        {
            return _ticketTypesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<TicketTypeDto> GetAsync(Guid id)
        {
            return _ticketTypesAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<TicketTypeDto> CreateAsync(TicketTypeCreateDto input)
        {
            return _ticketTypesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<TicketTypeDto> UpdateAsync(Guid id, TicketTypeUpdateDto input)
        {
            return _ticketTypesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _ticketTypesAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(TicketTypeExcelDownloadDto input)
        {
            return _ticketTypesAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _ticketTypesAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> tickettypeIds)
        {
            return _ticketTypesAppService.DeleteByIdsAsync(tickettypeIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetTicketTypesInput input)
        {
            return _ticketTypesAppService.DeleteAllAsync(input);
        }
    }
}