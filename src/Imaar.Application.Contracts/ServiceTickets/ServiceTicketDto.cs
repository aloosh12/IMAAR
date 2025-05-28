using Imaar.ServiceTickets;
using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Imaar.ServiceTickets
{
    public abstract class ServiceTicketDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Description { get; set; } = null!;
        public TicketEntityType TicketEntityType { get; set; }
        public string TicketEntityId { get; set; } = null!;
        public Guid ServiceTicketTypeId { get; set; }
        public Guid TicketCreatorId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}