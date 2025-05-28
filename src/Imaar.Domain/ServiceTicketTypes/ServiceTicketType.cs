using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace Imaar.ServiceTicketTypes
{
    public abstract class ServiceTicketTypeBase : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Title { get; set; }

        public virtual int? Order { get; set; }

        public virtual bool IsActive { get; set; }

        protected ServiceTicketTypeBase()
        {

        }

        public ServiceTicketTypeBase(Guid id, string title, bool isActive, int? order = null)
        {

            Id = id;
            Check.NotNull(title, nameof(title));
            Title = title;
            IsActive = isActive;
            Order = order;
        }

    }
}