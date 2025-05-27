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

namespace Imaar.UserFollows
{
    public abstract class UserFollowBase : FullAuditedAggregateRoot<Guid>
    {

        public Guid FollowerUserId { get; set; }
        public Guid FollowingUserId { get; set; }

        protected UserFollowBase()
        {

        }

        public UserFollowBase(Guid id, Guid followerUserId, Guid followingUserId)
        {

            Id = id;
            FollowerUserId = followerUserId;
            FollowingUserId = followingUserId;
        }

    }
}