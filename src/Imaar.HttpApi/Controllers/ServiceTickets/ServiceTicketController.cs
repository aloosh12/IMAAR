using Imaar.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.ServiceTickets;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.ServiceTickets
{
    [RemoteService]
    [Area("app")]
    [ControllerName("ServiceTicket")]
    [Route("api/app/service-tickets")]

    public abstract class ServiceTicketControllerBase : AbpController
    {
        protected IServiceTicketsAppService _serviceTicketsAppService;

        public ServiceTicketControllerBase(IServiceTicketsAppService serviceTicketsAppService)
        {
            _serviceTicketsAppService = serviceTicketsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<ServiceTicketWithNavigationPropertiesDto>> GetListAsync(GetServiceTicketsInput input)
        {
            return _serviceTicketsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<ServiceTicketWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _serviceTicketsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<ServiceTicketDto> GetAsync(Guid id)
        {
            return _serviceTicketsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("service-ticket-type-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetServiceTicketTypeLookupAsync(LookupRequestDto input)
        {
            return _serviceTicketsAppService.GetServiceTicketTypeLookupAsync(input);
        }

        [HttpGet]
        [Route("user-profile-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input)
        {
            return _serviceTicketsAppService.GetUserProfileLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<ServiceTicketDto> CreateAsync(ServiceTicketCreateDto input)
        {
            return _serviceTicketsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<ServiceTicketDto> UpdateAsync(Guid id, ServiceTicketUpdateDto input)
        {
            return _serviceTicketsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _serviceTicketsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(ServiceTicketExcelDownloadDto input)
        {
            return _serviceTicketsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _serviceTicketsAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> serviceticketIds)
        {
            return _serviceTicketsAppService.DeleteByIdsAsync(serviceticketIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetServiceTicketsInput input)
        {
            return _serviceTicketsAppService.DeleteAllAsync(input);
        }
    }
}