using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.TicketTypes;

namespace Imaar.Controllers.TicketTypes
{
    [RemoteService]
    [Area("app")]
    [ControllerName("TicketType")]
    [Route("api/mobile/ticket-types")]

    public class TicketTypeController : TicketTypeControllerBase, ITicketTypesAppService
    {
        public TicketTypeController(ITicketTypesAppService ticketTypesAppService) : base(ticketTypesAppService)
        {
        }
    }
}