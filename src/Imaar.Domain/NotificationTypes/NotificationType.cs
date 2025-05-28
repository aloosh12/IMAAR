using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace Imaar.NotificationTypes
{
    public abstract class NotificationTypeBase : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Title { get; set; }

        protected NotificationTypeBase()
        {

        }

        public NotificationTypeBase(Guid id, string title)
        {

            Id = id;
            Check.NotNull(title, nameof(title));
            Title = title;
        }

    }
}