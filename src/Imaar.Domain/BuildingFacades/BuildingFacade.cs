using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace Imaar.BuildingFacades
{
    public abstract class BuildingFacadeBase : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Name { get; set; }

        public virtual int? Order { get; set; }

        public virtual bool IsActive { get; set; }

        protected BuildingFacadeBase()
        {

        }

        public BuildingFacadeBase(Guid id, string name, bool isActive, int? order = null)
        {

            Id = id;
            Check.NotNull(name, nameof(name));
            Name = name;
            IsActive = isActive;
            Order = order;
        }

    }
}