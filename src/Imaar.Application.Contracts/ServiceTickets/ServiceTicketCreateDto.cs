using Imaar.ServiceTickets;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.ServiceTickets
{
    public abstract class ServiceTicketCreateDtoBase
    {
        [Required]
        public string Description { get; set; } = null!;
        public TicketEntityType TicketEntityType { get; set; } = ((TicketEntityType[])Enum.GetValues(typeof(TicketEntityType)))[0];
        [Required]
        public string TicketEntityId { get; set; } = null!;
        public Guid ServiceTicketTypeId { get; set; }
        public Guid TicketCreatorId { get; set; }
    }
}