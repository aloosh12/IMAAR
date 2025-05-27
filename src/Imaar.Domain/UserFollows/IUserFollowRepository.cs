using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.UserFollows
{
    public partial interface IUserFollowRepository : IRepository<UserFollow, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            Guid? followerUserId = null,
            Guid? followingUserId = null,
            CancellationToken cancellationToken = default);
        Task<UserFollowWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<UserFollowWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            Guid? followerUserId = null,
            Guid? followingUserId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<UserFollow>> GetListAsync(
                    string? filterText = null,

                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            Guid? followerUserId = null,
            Guid? followingUserId = null,
            CancellationToken cancellationToken = default);
    }
}