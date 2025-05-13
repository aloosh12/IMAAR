using Imaar.TicketTypes;
using Imaar.UserProfiles;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace Imaar.Tickets
{
    public abstract class TicketBase : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Description { get; set; }
        public Guid TicketTypeId { get; set; }
        public Guid TicketCreatorId { get; set; }
        public Guid TicketAgainstId { get; set; }

        protected TicketBase()
        {

        }

        public TicketBase(Guid id, Guid ticketTypeId, Guid ticketCreatorId, Guid ticketAgainstId, string description)
        {

            Id = id;
            Check.NotNull(description, nameof(description));
            Description = description;
            TicketTypeId = ticketTypeId;
            TicketCreatorId = ticketCreatorId;
            TicketAgainstId = ticketAgainstId;
        }

    }
}