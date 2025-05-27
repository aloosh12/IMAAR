using Imaar.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Imaar.EntityFrameworkCore;

namespace Imaar.UserFollows
{
    public abstract class EfCoreUserFollowRepositoryBase : EfCoreRepository<ImaarDbContext, UserFollow, Guid>
    {
        public EfCoreUserFollowRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        Guid? followerUserId = null,
            Guid? followingUserId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, followerUserId, followingUserId);

            var ids = query.Select(x => x.UserFollow.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<UserFollowWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(userFollow => new UserFollowWithNavigationProperties
                {
                    UserFollow = userFollow,
                    FollowerUser = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == userFollow.FollowerUserId),
                    FollowingUser = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == userFollow.FollowingUserId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<UserFollowWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            Guid? followerUserId = null,
            Guid? followingUserId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, followerUserId, followingUserId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserFollowConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<UserFollowWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from userFollow in (await GetDbSetAsync())
                   join followerUser in (await GetDbContextAsync()).Set<UserProfile>() on userFollow.FollowerUserId equals followerUser.Id into userProfiles
                   from followerUser in userProfiles.DefaultIfEmpty()
                   join followingUser in (await GetDbContextAsync()).Set<UserProfile>() on userFollow.FollowingUserId equals followingUser.Id into userProfiles1
                   from followingUser in userProfiles1.DefaultIfEmpty()
                   select new UserFollowWithNavigationProperties
                   {
                       UserFollow = userFollow,
                       FollowerUser = followerUser,
                       FollowingUser = followingUser
                   };
        }

        protected virtual IQueryable<UserFollowWithNavigationProperties> ApplyFilter(
            IQueryable<UserFollowWithNavigationProperties> query,
            string? filterText,
            Guid? followerUserId = null,
            Guid? followingUserId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => true)
                    .WhereIf(followerUserId != null && followerUserId != Guid.Empty, e => e.FollowerUser != null && e.FollowerUser.Id == followerUserId)
                    .WhereIf(followingUserId != null && followingUserId != Guid.Empty, e => e.FollowingUser != null && e.FollowingUser.Id == followingUserId);
        }

        public virtual async Task<List<UserFollow>> GetListAsync(
            string? filterText = null,

            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserFollowConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            Guid? followerUserId = null,
            Guid? followingUserId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, followerUserId, followingUserId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<UserFollow> ApplyFilter(
            IQueryable<UserFollow> query,
            string? filterText = null
)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => true)
;
        }
    }
}