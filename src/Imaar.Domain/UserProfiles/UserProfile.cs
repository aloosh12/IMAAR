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

namespace Imaar.UserProfiles
{
    public abstract class UserProfileBase : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string SecurityNumber { get; set; }

        public virtual BiologicalSex? BiologicalSex { get; set; }

        public virtual DateOnly? DateOfBirth { get; set; }

        [CanBeNull]
        public virtual string? Latitude { get; set; }

        [CanBeNull]
        public virtual string? Longitude { get; set; }

        [CanBeNull]
        public virtual string? ProfilePhoto { get; set; }

        protected UserProfileBase()
        {

        }

        public UserProfileBase(Guid id, string securityNumber, BiologicalSex? biologicalSex = null, DateOnly? dateOfBirth = null, string? latitude = null, string? longitude = null, string? profilePhoto = null)
        {

            Id = id;
            Check.NotNull(securityNumber, nameof(securityNumber));
            SecurityNumber = securityNumber;
            BiologicalSex = biologicalSex;
            DateOfBirth = dateOfBirth;
            Latitude = latitude;
            Longitude = longitude;
            ProfilePhoto = profilePhoto;
        }

    }
}