using Imaar.Medias;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace Imaar.Medias
{
    public abstract class MediaBase : FullAuditedAggregateRoot<Guid>
    {
        [CanBeNull]
        public virtual string? Title { get; set; }

        [NotNull]
        public virtual string File { get; set; }

        public virtual int Order { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual MediaEntityType SourceEntityType { get; set; }

        [NotNull]
        public virtual string SourceEntityId { get; set; }

        protected MediaBase()
        {

        }

        public MediaBase(Guid id, string file, int order, bool isActive, MediaEntityType sourceEntityType, string sourceEntityId, string? title = null)
        {

            Id = id;
            Check.NotNull(file, nameof(file));
            Check.NotNull(sourceEntityId, nameof(sourceEntityId));
            File = file;
            Order = order;
            IsActive = isActive;
            SourceEntityType = sourceEntityType;
            SourceEntityId = sourceEntityId;
            Title = title;
        }

    }
}