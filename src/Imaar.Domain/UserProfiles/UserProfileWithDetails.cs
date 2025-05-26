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
using Imaar.UserWorksExhibitions;
using Volo.Abp.Identity;

namespace Imaar.UserProfiles
{
    public class UserProfileWithDetails : FullAuditedAggregateRoot<Guid>
    {
       
        public virtual UserProfile UserProfile { get; set; }
        public virtual IdentityUser IdentityUser { get; set; }
        public virtual List<UserWorksExhibition> UserWorksExhibitionList { get; set; }
        public virtual string Role { get; set; }
        public double SpeedOfCompletion { get; set; }
        public double Dealing { get; set; }
        public double Cleanliness { get; set; }
        public double Perfection { get; set; }
        public double Price { get; set; }
        public UserProfileWithDetails()
        {

        }

   

    }
}