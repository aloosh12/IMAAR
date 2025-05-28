using Imaar.ServiceTickets;
using Imaar.ServiceTicketTypes;
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

namespace Imaar.ServiceTickets
{
    public abstract class ServiceTicketBase : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Description { get; set; }

        public virtual TicketEntityType TicketEntityType { get; set; }

        [NotNull]
        public virtual string TicketEntityId { get; set; }
        public Guid ServiceTicketTypeId { get; set; }
        public Guid TicketCreatorId { get; set; }

        protected ServiceTicketBase()
        {

        }

        public ServiceTicketBase(Guid id, Guid serviceTicketTypeId, Guid ticketCreatorId, string description, TicketEntityType ticketEntityType, string ticketEntityId)
        {

            Id = id;
            Check.NotNull(description, nameof(description));
            Check.NotNull(ticketEntityId, nameof(ticketEntityId));
            Description = description;
            TicketEntityType = ticketEntityType;
            TicketEntityId = ticketEntityId;
            ServiceTicketTypeId = serviceTicketTypeId;
            TicketCreatorId = ticketCreatorId;
        }

    }
}