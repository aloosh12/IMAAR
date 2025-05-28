using Imaar.ServiceTickets;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Imaar.ServiceTickets
{
    public abstract class ServiceTicketUpdateDtoBase : IHasConcurrencyStamp
    {
        [Required]
        public string Description { get; set; } = null!;
        public TicketEntityType TicketEntityType { get; set; }
        [Required]
        public string TicketEntityId { get; set; } = null!;
        public Guid ServiceTicketTypeId { get; set; }
        public Guid TicketCreatorId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}