using Imaar.ServiceTickets;
using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.ServiceTickets
{
    public abstract class GetServiceTicketsInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Description { get; set; }
        public TicketEntityType? TicketEntityType { get; set; }
        public string? TicketEntityId { get; set; }
        public Guid? ServiceTicketTypeId { get; set; }
        public Guid? TicketCreatorId { get; set; }

        public GetServiceTicketsInputBase()
        {

        }
    }
}