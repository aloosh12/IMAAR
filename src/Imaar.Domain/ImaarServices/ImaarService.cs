using Imaar.ServiceTypes;
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

namespace Imaar.ImaarServices
{
    public abstract class ImaarServiceBase : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Title { get; set; }

        [NotNull]
        public virtual string Description { get; set; }

        [NotNull]
        public virtual string ServiceLocation { get; set; }

        [NotNull]
        public virtual string ServiceNumber { get; set; }

        public virtual DateOnly DateOfPublish { get; set; }

        public virtual int Price { get; set; }

        [CanBeNull]
        public virtual string? Latitude { get; set; }

        [CanBeNull]
        public virtual string? Longitude { get; set; }
        public Guid ServiceTypeId { get; set; }
        public Guid UserProfileId { get; set; }

        protected ImaarServiceBase()
        {

        }

        public ImaarServiceBase(Guid id, Guid serviceTypeId, Guid userProfileId, string title, string description, string serviceLocation, string serviceNumber, DateOnly dateOfPublish, int price, string? latitude = null, string? longitude = null)
        {

            Id = id;
            Check.NotNull(title, nameof(title));
            Check.NotNull(description, nameof(description));
            Check.NotNull(serviceLocation, nameof(serviceLocation));
            Check.NotNull(serviceNumber, nameof(serviceNumber));
            Title = title;
            Description = description;
            ServiceLocation = serviceLocation;
            ServiceNumber = serviceNumber;
            DateOfPublish = dateOfPublish;
            Price = price;
            Latitude = latitude;
            Longitude = longitude;
            ServiceTypeId = serviceTypeId;
            UserProfileId = userProfileId;
        }

    }
}