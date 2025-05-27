using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.UserFollows
{
    public abstract class UserFollowCreateDtoBase
    {

        public Guid FollowerUserId { get; set; }
        public Guid FollowingUserId { get; set; }
    }
}