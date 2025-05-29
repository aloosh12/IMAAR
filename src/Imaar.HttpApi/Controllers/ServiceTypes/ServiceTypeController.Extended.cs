using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.ServiceTypes;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace Imaar.Controllers.ServiceTypes
{
    [RemoteService]
    [Area("app")]
    [ControllerName("ServiceType")]
    [Route("api/mobile/service-types")]

    public class ServiceTypeController : ServiceTypeControllerBase, IServiceTypesAppService
    {
        public ServiceTypeController(IServiceTypesAppService serviceTypesAppService) : base(serviceTypesAppService)
        {
        }
        
        [HttpGet("building-service-type")]
        [AllowAnonymous]
        public Task<List<ServiceTypeDto>> GetBuildingServiceTypesByCategoryAsync()
        {
            return _serviceTypesAppService.GetBuildingServiceTypesByCategoryAsync();
        }
        
        [HttpGet("vacancies-service-type")]
        [AllowAnonymous]
        public Task<List<ServiceTypeDto>> GetVacancyServiceTypesAsync()
        {
            return _serviceTypesAppService.GetVacancyServiceTypesAsync();
        }
        
        [HttpGet("imaar-services-service-type")]
        [AllowAnonymous]
        public Task<List<ServiceTypeDto>> GetImaarServiceTypesAsync()
        {
            return _serviceTypesAppService.GetImaarServiceTypesAsync();
        }
    }
}