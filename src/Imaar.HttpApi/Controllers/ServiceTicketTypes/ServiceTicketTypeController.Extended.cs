using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.ServiceTicketTypes;

namespace Imaar.Controllers.ServiceTicketTypes
{
    [RemoteService]
    [Area("app")]
    [ControllerName("ServiceTicketType")]
    [Route("api/app/service-ticket-types")]

    public class ServiceTicketTypeController : ServiceTicketTypeControllerBase, IServiceTicketTypesAppService
    {
        public ServiceTicketTypeController(IServiceTicketTypesAppService serviceTicketTypesAppService) : base(serviceTicketTypesAppService)
        {
        }
    }
}