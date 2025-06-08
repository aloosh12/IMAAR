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

namespace Imaar.Advertisements
{
    public abstract class AdvertisementBase : FullAuditedAggregateRoot<Guid>
    {
        [CanBeNull]
        public virtual string? Title { get; set; }

        [CanBeNull]
        public virtual string? SubTitle { get; set; }

        [NotNull]
        public virtual string File { get; set; }

        public virtual DateTime? FromDateTime { get; set; }

        public virtual DateTime? ToDateTime { get; set; }

        public virtual int Order { get; set; }

        public virtual bool IsActive { get; set; }
        public Guid? UserProfileId { get; set; }

        protected AdvertisementBase()
        {

        }

        public AdvertisementBase(Guid id, Guid? userProfileId, string file, int order, bool isActive, string? title = null, string? subTitle = null, DateTime? fromDateTime = null, DateTime? toDateTime = null)
        {

            Id = id;
            Check.NotNull(file, nameof(file));
            File = file;
            Order = order;
            IsActive = isActive;
            Title = title;
            SubTitle = subTitle;
            FromDateTime = fromDateTime;
            ToDateTime = toDateTime;
            UserProfileId = userProfileId;
        }

    }
}