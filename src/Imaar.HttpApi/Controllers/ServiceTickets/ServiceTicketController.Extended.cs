using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.ServiceTickets;

namespace Imaar.Controllers.ServiceTickets
{
    [RemoteService]
    [Area("app")]
    [ControllerName("ServiceTicket")]
    [Route("api/mobile/service-tickets")]

    public class ServiceTicketController : ServiceTicketControllerBase, IServiceTicketsAppService
    {
        public ServiceTicketController(IServiceTicketsAppService serviceTicketsAppService) : base(serviceTicketsAppService)
        {
        }
    }
}