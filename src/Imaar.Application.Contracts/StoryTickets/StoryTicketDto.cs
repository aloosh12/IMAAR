using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Imaar.StoryTickets
{
    public abstract class StoryTicketDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Description { get; set; } = null!;
        public Guid StoryTicketTypeId { get; set; }
        public Guid TicketCreatorId { get; set; }
        public Guid StoryAgainstId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}