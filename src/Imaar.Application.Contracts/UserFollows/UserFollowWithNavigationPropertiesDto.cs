using Imaar.UserProfiles;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace Imaar.UserFollows
{
    public abstract class UserFollowWithNavigationPropertiesDtoBase
    {
        public UserFollowDto UserFollow { get; set; } = null!;

        public UserProfileDto FollowerUser { get; set; } = null!;
        public UserProfileDto FollowingUser { get; set; } = null!;

    }
}