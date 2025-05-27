using Imaar.Cities;
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

namespace Imaar.Regions
{
    public abstract class EfCoreRegionRepositoryBase : EfCoreRepository<ImaarDbContext, Region, Guid>
    {
        public EfCoreRegionRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? name = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? cityId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, name, orderMin, orderMax, isActive, cityId);

            var ids = query.Select(x => x.Region.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<RegionWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(region => new RegionWithNavigationProperties
                {
                    Region = region,
                    City = dbContext.Set<City>().FirstOrDefault(c => c.Id == region.CityId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<RegionWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? name = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? cityId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, name, orderMin, orderMax, isActive, cityId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? RegionConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<RegionWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from region in (await GetDbSetAsync())
                   join city in (await GetDbContextAsync()).Set<City>() on region.CityId equals city.Id into cities
                   from city in cities.DefaultIfEmpty()
                   select new RegionWithNavigationProperties
                   {
                       Region = region,
                       City = city
                   };
        }

        protected virtual IQueryable<RegionWithNavigationProperties> ApplyFilter(
            IQueryable<RegionWithNavigationProperties> query,
            string? filterText,
            string? name = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? cityId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Region.Name!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Region.Name.Contains(name))
                    .WhereIf(orderMin.HasValue, e => e.Region.Order >= orderMin!.Value)
                    .WhereIf(orderMax.HasValue, e => e.Region.Order <= orderMax!.Value)
                    .WhereIf(isActive.HasValue, e => e.Region.IsActive == isActive)
                    .WhereIf(cityId != null && cityId != Guid.Empty, e => e.City != null && e.City.Id == cityId);
        }

        public virtual async Task<List<Region>> GetListAsync(
            string? filterText = null,
            string? name = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, name, orderMin, orderMax, isActive);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? RegionConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? cityId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, name, orderMin, orderMax, isActive, cityId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Region> ApplyFilter(
            IQueryable<Region> query,
            string? filterText = null,
            string? name = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin!.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax!.Value)
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive);
        }
    }
}