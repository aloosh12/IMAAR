using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace Imaar.ServiceTypes
{
    public abstract class ServiceTypeBase : FullAuditedEntity<Guid>
    {
        public virtual Guid CategoryId { get; set; }

        [NotNull]
        public virtual string Title { get; set; }

        [CanBeNull]
        public virtual string? Icon { get; set; }

        public virtual int Order { get; set; }

        public virtual bool IsActive { get; set; }

        protected ServiceTypeBase()
        {

        }

        public ServiceTypeBase(Guid id, Guid categoryId, string title, int order, bool isActive, string? icon = null)
        {

            Id = id;
            Check.NotNull(title, nameof(title));
            CategoryId = categoryId;
            Title = title;
            Order = order;
            IsActive = isActive;
            Icon = icon;
        }

    }
}