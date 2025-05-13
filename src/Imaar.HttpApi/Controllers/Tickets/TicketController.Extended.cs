using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.Tickets;

namespace Imaar.Controllers.Tickets
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Ticket")]
    [Route("api/mobile/tickets")]

    public class TicketController : TicketControllerBase, ITicketsAppService
    {
        public TicketController(ITicketsAppService ticketsAppService) : base(ticketsAppService)
        {
        }
    }
}