using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.ServiceTypes;

namespace Imaar.Controllers.ServiceTypes
{
    [RemoteService]
    [Area("app")]
    [ControllerName("ServiceType")]
    [Route("api/app/service-types")]

    public abstract class ServiceTypeControllerBase : AbpController
    {
        protected IServiceTypesAppService _serviceTypesAppService;

        public ServiceTypeControllerBase(IServiceTypesAppService serviceTypesAppService)
        {
            _serviceTypesAppService = serviceTypesAppService;
        }

        [HttpGet]
        [Route("by-category")]
        public virtual Task<PagedResultDto<ServiceTypeDto>> GetListByCategoryIdAsync(GetServiceTypeListInput input)
        {
            return _serviceTypesAppService.GetListByCategoryIdAsync(input);
        }

        [HttpGet]
        public virtual Task<PagedResultDto<ServiceTypeDto>> GetListAsync(GetServiceTypesInput input)
        {
            return _serviceTypesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<ServiceTypeDto> GetAsync(Guid id)
        {
            return _serviceTypesAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<ServiceTypeDto> CreateAsync(ServiceTypeCreateDto input)
        {
            return _serviceTypesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<ServiceTypeDto> UpdateAsync(Guid id, ServiceTypeUpdateDto input)
        {
            return _serviceTypesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _serviceTypesAppService.DeleteAsync(id);
        }
    }
}