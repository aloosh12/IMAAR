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

namespace Imaar.Stories
{
    public abstract class StoryBase : FullAuditedAggregateRoot<Guid>
    {
        [CanBeNull]
        public virtual string? Title { get; set; }

        public virtual DateTime FromTime { get; set; }

        public virtual DateTime ExpiryTime { get; set; }
        public Guid? StoryPublisherId { get; set; }

        protected StoryBase()
        {

        }

        public StoryBase(Guid id, Guid? storyPublisherId, DateTime fromTime, DateTime expiryTime, string? title = null)
        {

            Id = id;
            FromTime = fromTime;
            ExpiryTime = expiryTime;
            Title = title;
            StoryPublisherId = storyPublisherId;
        }

    }
}