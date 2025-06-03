using Imaar.StoryTicketTypes;
using Imaar.UserProfiles;
using Imaar.Stories;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace Imaar.StoryTickets
{
    public abstract class StoryTicketBase : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Description { get; set; }
        public Guid StoryTicketTypeId { get; set; }
        public Guid TicketCreatorId { get; set; }
        public Guid StoryAgainstId { get; set; }

        protected StoryTicketBase()
        {

        }

        public StoryTicketBase(Guid id, Guid storyTicketTypeId, Guid ticketCreatorId, Guid storyAgainstId, string description)
        {

            Id = id;
            Check.NotNull(description, nameof(description));
            Description = description;
            StoryTicketTypeId = storyTicketTypeId;
            TicketCreatorId = ticketCreatorId;
            StoryAgainstId = storyAgainstId;
        }

    }
}