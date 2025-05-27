using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Imaar.UserFollows
{
    public abstract class UserFollowDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {

        public Guid FollowerUserId { get; set; }
        public Guid FollowingUserId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}