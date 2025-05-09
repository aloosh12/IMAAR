using Asp.Versioning;
using System;
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
    [Route("api/mobile/service-types")]

    public class ServiceTypeController : ServiceTypeControllerBase, IServiceTypesAppService
    {
        public ServiceTypeController(IServiceTypesAppService serviceTypesAppService) : base(serviceTypesAppService)
        {
        }
    }
}