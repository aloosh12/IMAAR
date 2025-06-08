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

namespace Imaar.Advertisements
{
    public abstract class EfCoreAdvertisementRepositoryBase : EfCoreRepository<ImaarDbContext, Advertisement, Guid>
    {
        public EfCoreAdvertisementRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? title = null,
            string? subTitle = null,
            string? file = null,
            DateTime? fromDateTimeMin = null,
            DateTime? fromDateTimeMax = null,
            DateTime? toDateTimeMin = null,
            DateTime? toDateTimeMax = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? userProfileId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, title, subTitle, file, fromDateTimeMin, fromDateTimeMax, toDateTimeMin, toDateTimeMax, orderMin, orderMax, isActive, userProfileId);

            var ids = query.Select(x => x.Advertisement.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<AdvertisementWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(advertisement => new AdvertisementWithNavigationProperties
                {
                    Advertisement = advertisement,
                    UserProfile = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == advertisement.UserProfileId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<AdvertisementWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? title = null,
            string? subTitle = null,
            string? file = null,
            DateTime? fromDateTimeMin = null,
            DateTime? fromDateTimeMax = null,
            DateTime? toDateTimeMin = null,
            DateTime? toDateTimeMax = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? userProfileId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, title, subTitle, file, fromDateTimeMin, fromDateTimeMax, toDateTimeMin, toDateTimeMax, orderMin, orderMax, isActive, userProfileId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? AdvertisementConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<AdvertisementWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from advertisement in (await GetDbSetAsync())
                   join userProfile in (await GetDbContextAsync()).Set<UserProfile>() on advertisement.UserProfileId equals userProfile.Id into userProfiles
                   from userProfile in userProfiles.DefaultIfEmpty()
                   select new AdvertisementWithNavigationProperties
                   {
                       Advertisement = advertisement,
                       UserProfile = userProfile
                   };
        }

        protected virtual IQueryable<AdvertisementWithNavigationProperties> ApplyFilter(
            IQueryable<AdvertisementWithNavigationProperties> query,
            string? filterText,
            string? title = null,
            string? subTitle = null,
            string? file = null,
            DateTime? fromDateTimeMin = null,
            DateTime? fromDateTimeMax = null,
            DateTime? toDateTimeMin = null,
            DateTime? toDateTimeMax = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? userProfileId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Advertisement.Title!.Contains(filterText!) || e.Advertisement.SubTitle!.Contains(filterText!) || e.Advertisement.File!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(title), e => e.Advertisement.Title.Contains(title))
                    .WhereIf(!string.IsNullOrWhiteSpace(subTitle), e => e.Advertisement.SubTitle.Contains(subTitle))
                    .WhereIf(!string.IsNullOrWhiteSpace(file), e => e.Advertisement.File.Contains(file))
                    .WhereIf(fromDateTimeMin.HasValue, e => e.Advertisement.FromDateTime >= fromDateTimeMin!.Value)
                    .WhereIf(fromDateTimeMax.HasValue, e => e.Advertisement.FromDateTime <= fromDateTimeMax!.Value)
                    .WhereIf(toDateTimeMin.HasValue, e => e.Advertisement.ToDateTime >= toDateTimeMin!.Value)
                    .WhereIf(toDateTimeMax.HasValue, e => e.Advertisement.ToDateTime <= toDateTimeMax!.Value)
                    .WhereIf(orderMin.HasValue, e => e.Advertisement.Order >= orderMin!.Value)
                    .WhereIf(orderMax.HasValue, e => e.Advertisement.Order <= orderMax!.Value)
                    .WhereIf(isActive.HasValue, e => e.Advertisement.IsActive == isActive)
                    .WhereIf(userProfileId != null && userProfileId != Guid.Empty, e => e.UserProfile != null && e.UserProfile.Id == userProfileId);
        }

        public virtual async Task<List<Advertisement>> GetListAsync(
            string? filterText = null,
            string? title = null,
            string? subTitle = null,
            string? file = null,
            DateTime? fromDateTimeMin = null,
            DateTime? fromDateTimeMax = null,
            DateTime? toDateTimeMin = null,
            DateTime? toDateTimeMax = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, title, subTitle, file, fromDateTimeMin, fromDateTimeMax, toDateTimeMin, toDateTimeMax, orderMin, orderMax, isActive);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? AdvertisementConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? title = null,
            string? subTitle = null,
            string? file = null,
            DateTime? fromDateTimeMin = null,
            DateTime? fromDateTimeMax = null,
            DateTime? toDateTimeMin = null,
            DateTime? toDateTimeMax = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? userProfileId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, title, subTitle, file, fromDateTimeMin, fromDateTimeMax, toDateTimeMin, toDateTimeMax, orderMin, orderMax, isActive, userProfileId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Advertisement> ApplyFilter(
            IQueryable<Advertisement> query,
            string? filterText = null,
            string? title = null,
            string? subTitle = null,
            string? file = null,
            DateTime? fromDateTimeMin = null,
            DateTime? fromDateTimeMax = null,
            DateTime? toDateTimeMin = null,
            DateTime? toDateTimeMax = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Title!.Contains(filterText!) || e.SubTitle!.Contains(filterText!) || e.File!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(title), e => e.Title.Contains(title))
                    .WhereIf(!string.IsNullOrWhiteSpace(subTitle), e => e.SubTitle.Contains(subTitle))
                    .WhereIf(!string.IsNullOrWhiteSpace(file), e => e.File.Contains(file))
                    .WhereIf(fromDateTimeMin.HasValue, e => e.FromDateTime >= fromDateTimeMin!.Value)
                    .WhereIf(fromDateTimeMax.HasValue, e => e.FromDateTime <= fromDateTimeMax!.Value)
                    .WhereIf(toDateTimeMin.HasValue, e => e.ToDateTime >= toDateTimeMin!.Value)
                    .WhereIf(toDateTimeMax.HasValue, e => e.ToDateTime <= toDateTimeMax!.Value)
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin!.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax!.Value)
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive);
        }
    }
}