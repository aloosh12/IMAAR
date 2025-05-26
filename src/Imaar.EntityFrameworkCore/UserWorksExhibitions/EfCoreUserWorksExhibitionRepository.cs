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

namespace Imaar.UserWorksExhibitions
{
    public abstract class EfCoreUserWorksExhibitionRepositoryBase : EfCoreRepository<ImaarDbContext, UserWorksExhibition, Guid>
    {
        public EfCoreUserWorksExhibitionRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? title = null,
            string? file = null,
            int? orderMin = null,
            int? orderMax = null,
            Guid? userProfileId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, title, file, orderMin, orderMax, userProfileId);

            var ids = query.Select(x => x.UserWorksExhibition.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<UserWorksExhibitionWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(userWorksExhibition => new UserWorksExhibitionWithNavigationProperties
                {
                    UserWorksExhibition = userWorksExhibition,
                    UserProfile = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == userWorksExhibition.UserProfileId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<UserWorksExhibitionWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? title = null,
            string? file = null,
            int? orderMin = null,
            int? orderMax = null,
            Guid? userProfileId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, title, file, orderMin, orderMax, userProfileId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserWorksExhibitionConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<UserWorksExhibitionWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from userWorksExhibition in (await GetDbSetAsync())
                   join userProfile in (await GetDbContextAsync()).Set<UserProfile>() on userWorksExhibition.UserProfileId equals userProfile.Id into userProfiles
                   from userProfile in userProfiles.DefaultIfEmpty()
                   select new UserWorksExhibitionWithNavigationProperties
                   {
                       UserWorksExhibition = userWorksExhibition,
                       UserProfile = userProfile
                   };
        }

        protected virtual IQueryable<UserWorksExhibitionWithNavigationProperties> ApplyFilter(
            IQueryable<UserWorksExhibitionWithNavigationProperties> query,
            string? filterText,
            string? title = null,
            string? file = null,
            int? orderMin = null,
            int? orderMax = null,
            Guid? userProfileId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.UserWorksExhibition.Title!.Contains(filterText!) || e.UserWorksExhibition.File!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(title), e => e.UserWorksExhibition.Title.Contains(title))
                    .WhereIf(!string.IsNullOrWhiteSpace(file), e => e.UserWorksExhibition.File.Contains(file))
                    .WhereIf(orderMin.HasValue, e => e.UserWorksExhibition.Order >= orderMin!.Value)
                    .WhereIf(orderMax.HasValue, e => e.UserWorksExhibition.Order <= orderMax!.Value)
                    .WhereIf(userProfileId != null && userProfileId != Guid.Empty, e => e.UserProfile != null && e.UserProfile.Id == userProfileId);
        }

        public virtual async Task<List<UserWorksExhibition>> GetListAsync(
            string? filterText = null,
            string? title = null,
            string? file = null,
            int? orderMin = null,
            int? orderMax = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, title, file, orderMin, orderMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserWorksExhibitionConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? title = null,
            string? file = null,
            int? orderMin = null,
            int? orderMax = null,
            Guid? userProfileId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, title, file, orderMin, orderMax, userProfileId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<UserWorksExhibition> ApplyFilter(
            IQueryable<UserWorksExhibition> query,
            string? filterText = null,
            string? title = null,
            string? file = null,
            int? orderMin = null,
            int? orderMax = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Title!.Contains(filterText!) || e.File!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(title), e => e.Title.Contains(title))
                    .WhereIf(!string.IsNullOrWhiteSpace(file), e => e.File.Contains(file))
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin!.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax!.Value);
        }
    }
}