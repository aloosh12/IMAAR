using Imaar.ServiceTickets;
using System;

namespace Imaar.ServiceTickets
{
    public abstract class ServiceTicketExcelDtoBase
    {
        public string Description { get; set; } = null!;
        public TicketEntityType TicketEntityType { get; set; }
        public string TicketEntityId { get; set; } = null!;
    }
}