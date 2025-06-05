using Imaar.UserSavedItems;
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

namespace Imaar.UserSavedItems
{
    public abstract class EfCoreUserSavedItemRepositoryBase : EfCoreRepository<ImaarDbContext, UserSavedItem, Guid>
    {
        public EfCoreUserSavedItemRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? sourceId = null,
            UserSavedItemType? savedItemType = null,
            Guid? userProfileId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, sourceId, savedItemType, userProfileId);

            var ids = query.Select(x => x.UserSavedItem.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<UserSavedItemWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(userSavedItem => new UserSavedItemWithNavigationProperties
                {
                    UserSavedItem = userSavedItem,
                    UserProfile = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == userSavedItem.UserProfileId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<UserSavedItemWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? sourceId = null,
            UserSavedItemType? savedItemType = null,
            Guid? userProfileId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, sourceId, savedItemType, userProfileId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserSavedItemConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<UserSavedItemWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from userSavedItem in (await GetDbSetAsync())
                   join userProfile in (await GetDbContextAsync()).Set<UserProfile>() on userSavedItem.UserProfileId equals userProfile.Id into userProfiles
                   from userProfile in userProfiles.DefaultIfEmpty()
                   select new UserSavedItemWithNavigationProperties
                   {
                       UserSavedItem = userSavedItem,
                       UserProfile = userProfile
                   };
        }

        protected virtual IQueryable<UserSavedItemWithNavigationProperties> ApplyFilter(
            IQueryable<UserSavedItemWithNavigationProperties> query,
            string? filterText,
            string? sourceId = null,
            UserSavedItemType? savedItemType = null,
            Guid? userProfileId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.UserSavedItem.SourceId!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(sourceId), e => e.UserSavedItem.SourceId.Contains(sourceId))
                    .WhereIf(savedItemType.HasValue, e => e.UserSavedItem.SavedItemType == savedItemType)
                    .WhereIf(userProfileId != null && userProfileId != Guid.Empty, e => e.UserProfile != null && e.UserProfile.Id == userProfileId);
        }

        public virtual async Task<List<UserSavedItem>> GetListAsync(
            string? filterText = null,
            string? sourceId = null,
            UserSavedItemType? savedItemType = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, sourceId, savedItemType);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserSavedItemConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? sourceId = null,
            UserSavedItemType? savedItemType = null,
            Guid? userProfileId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, sourceId, savedItemType, userProfileId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<UserSavedItem> ApplyFilter(
            IQueryable<UserSavedItem> query,
            string? filterText = null,
            string? sourceId = null,
            UserSavedItemType? savedItemType = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.SourceId!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(sourceId), e => e.SourceId.Contains(sourceId))
                    .WhereIf(savedItemType.HasValue, e => e.SavedItemType == savedItemType);
        }
    }
}