using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.UserFollows
{
    public abstract class UserFollowManagerBase : DomainService
    {
        protected IUserFollowRepository _userFollowRepository;

        public UserFollowManagerBase(IUserFollowRepository userFollowRepository)
        {
            _userFollowRepository = userFollowRepository;
        }

        public virtual async Task<UserFollow> CreateAsync(
        Guid followerUserId, Guid followingUserId)
        {
            Check.NotNull(followerUserId, nameof(followerUserId));
            Check.NotNull(followingUserId, nameof(followingUserId));

            var userFollow = new UserFollow(
             GuidGenerator.Create(),
             followerUserId, followingUserId
             );

            return await _userFollowRepository.InsertAsync(userFollow);
        }

        public virtual async Task<UserFollow> UpdateAsync(
            Guid id,
            Guid followerUserId, Guid followingUserId, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(followerUserId, nameof(followerUserId));
            Check.NotNull(followingUserId, nameof(followingUserId));

            var userFollow = await _userFollowRepository.GetAsync(id);

            userFollow.FollowerUserId = followerUserId;
            userFollow.FollowingUserId = followingUserId;

            userFollow.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _userFollowRepository.UpdateAsync(userFollow);
        }

    }
}