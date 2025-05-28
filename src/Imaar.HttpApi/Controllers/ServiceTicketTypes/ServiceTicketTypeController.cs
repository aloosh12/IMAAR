using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.ServiceTicketTypes;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.ServiceTicketTypes
{
    [RemoteService]
    [Area("app")]
    [ControllerName("ServiceTicketType")]
    [Route("api/app/service-ticket-types")]

    public abstract class ServiceTicketTypeControllerBase : AbpController
    {
        protected IServiceTicketTypesAppService _serviceTicketTypesAppService;

        public ServiceTicketTypeControllerBase(IServiceTicketTypesAppService serviceTicketTypesAppService)
        {
            _serviceTicketTypesAppService = serviceTicketTypesAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<ServiceTicketTypeDto>> GetListAsync(GetServiceTicketTypesInput input)
        {
            return _serviceTicketTypesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<ServiceTicketTypeDto> GetAsync(Guid id)
        {
            return _serviceTicketTypesAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<ServiceTicketTypeDto> CreateAsync(ServiceTicketTypeCreateDto input)
        {
            return _serviceTicketTypesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<ServiceTicketTypeDto> UpdateAsync(Guid id, ServiceTicketTypeUpdateDto input)
        {
            return _serviceTicketTypesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _serviceTicketTypesAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(ServiceTicketTypeExcelDownloadDto input)
        {
            return _serviceTicketTypesAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _serviceTicketTypesAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> servicetickettypeIds)
        {
            return _serviceTicketTypesAppService.DeleteByIdsAsync(servicetickettypeIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetServiceTicketTypesInput input)
        {
            return _serviceTicketTypesAppService.DeleteAllAsync(input);
        }
    }
}