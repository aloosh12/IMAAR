using Imaar.UserProfiles;
using Imaar.Stories;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace Imaar.StoryLovers
{
    public abstract class StoryLoverBase : FullAuditedAggregateRoot<Guid>
    {

        public Guid UserProfileId { get; set; }
        public Guid StoryId { get; set; }

        protected StoryLoverBase()
        {

        }

        public StoryLoverBase(Guid id, Guid userProfileId, Guid storyId)
        {

            Id = id;
            UserProfileId = userProfileId;
            StoryId = storyId;
        }

    }
}