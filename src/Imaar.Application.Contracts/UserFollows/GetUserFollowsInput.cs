using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.UserFollows
{
    public abstract class GetUserFollowsInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public Guid? FollowerUserId { get; set; }
        public Guid? FollowingUserId { get; set; }

        public GetUserFollowsInputBase()
        {

        }
    }
}