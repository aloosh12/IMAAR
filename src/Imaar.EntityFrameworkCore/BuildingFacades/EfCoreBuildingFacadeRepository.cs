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

namespace Imaar.BuildingFacades
{
    public abstract class EfCoreBuildingFacadeRepositoryBase : EfCoreRepository<ImaarDbContext, BuildingFacade, Guid>
    {
        public EfCoreBuildingFacadeRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? name = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default)
        {

            var query = await GetQueryableAsync();

            query = ApplyFilter(query, filterText, name, orderMin, orderMax, isActive);

            var ids = query.Select(x => x.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<BuildingFacade>> GetListAsync(
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
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? BuildingFacadeConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, name, orderMin, orderMax, isActive);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<BuildingFacade> ApplyFilter(
            IQueryable<BuildingFacade> query,
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