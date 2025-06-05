using Imaar.UserSavedItems;
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

namespace Imaar.UserSavedItems
{
    public abstract class UserSavedItemBase : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string SourceId { get; set; }

        public virtual UserSavedItemType SavedItemType { get; set; }
        public Guid UserProfileId { get; set; }

        protected UserSavedItemBase()
        {

        }

        public UserSavedItemBase(Guid id, Guid userProfileId, string sourceId, UserSavedItemType savedItemType)
        {

            Id = id;
            Check.NotNull(sourceId, nameof(sourceId));
            SourceId = sourceId;
            SavedItemType = savedItemType;
            UserProfileId = userProfileId;
        }

    }
}