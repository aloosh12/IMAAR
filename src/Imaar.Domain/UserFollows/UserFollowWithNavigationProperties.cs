using Imaar.UserProfiles;
using Imaar.UserProfiles;

using System;
using System.Collections.Generic;

namespace Imaar.UserFollows
{
    public abstract class UserFollowWithNavigationPropertiesBase
    {
        public UserFollow UserFollow { get; set; } = null!;

        public UserProfile FollowerUser { get; set; } = null!;
        public UserProfile FollowingUser { get; set; } = null!;
        

        
    }
}