using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Imaar.Tickets
{
    public abstract class TicketDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Description { get; set; } = null!;
        public Guid TicketTypeId { get; set; }
        public Guid TicketCreatorId { get; set; }
        public Guid TicketAgainstId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}