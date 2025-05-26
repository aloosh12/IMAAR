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

namespace Imaar.UserWorksExhibitions
{
    public abstract class UserWorksExhibitionBase : FullAuditedAggregateRoot<Guid>
    {
        [CanBeNull]
        public virtual string? Title { get; set; }

        [NotNull]
        public virtual string File { get; set; }

        public virtual int Order { get; set; }
        public Guid UserProfileId { get; set; }

        protected UserWorksExhibitionBase()
        {

        }

        public UserWorksExhibitionBase(Guid id, Guid userProfileId, string file, int order, string? title = null)
        {

            Id = id;
            Check.NotNull(file, nameof(file));
            File = file;
            Order = order;
            Title = title;
            UserProfileId = userProfileId;
        }

    }
}