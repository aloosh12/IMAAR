using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Imaar.UserFollows
{
    public abstract class UserFollowUpdateDtoBase : IHasConcurrencyStamp
    {

        public Guid FollowerUserId { get; set; }
        public Guid FollowingUserId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}