using Imaar.Buildings;
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

namespace Imaar.BuildingEvaluations
{
    public abstract class EfCoreBuildingEvaluationRepositoryBase : EfCoreRepository<ImaarDbContext, BuildingEvaluation, Guid>
    {
        public EfCoreBuildingEvaluationRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        int? rateMin = null,
            int? rateMax = null,
            Guid? evaluatorId = null,
            Guid? buildingId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, rateMin, rateMax, evaluatorId, buildingId);

            var ids = query.Select(x => x.BuildingEvaluation.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<BuildingEvaluationWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(buildingEvaluation => new BuildingEvaluationWithNavigationProperties
                {
                    BuildingEvaluation = buildingEvaluation,
                    Evaluator = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == buildingEvaluation.EvaluatorId),
                    Building = dbContext.Set<Building>().FirstOrDefault(c => c.Id == buildingEvaluation.BuildingId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<BuildingEvaluationWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            Guid? evaluatorId = null,
            Guid? buildingId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, rateMin, rateMax, evaluatorId, buildingId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? BuildingEvaluationConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<BuildingEvaluationWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from buildingEvaluation in (await GetDbSetAsync())
                   join evaluator in (await GetDbContextAsync()).Set<UserProfile>() on buildingEvaluation.EvaluatorId equals evaluator.Id into userProfiles
                   from evaluator in userProfiles.DefaultIfEmpty()
                   join building in (await GetDbContextAsync()).Set<Building>() on buildingEvaluation.BuildingId equals building.Id into buildings
                   from building in buildings.DefaultIfEmpty()
                   select new BuildingEvaluationWithNavigationProperties
                   {
                       BuildingEvaluation = buildingEvaluation,
                       Evaluator = evaluator,
                       Building = building
                   };
        }

        protected virtual IQueryable<BuildingEvaluationWithNavigationProperties> ApplyFilter(
            IQueryable<BuildingEvaluationWithNavigationProperties> query,
            string? filterText,
            int? rateMin = null,
            int? rateMax = null,
            Guid? evaluatorId = null,
            Guid? buildingId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => true)
                    .WhereIf(rateMin.HasValue, e => e.BuildingEvaluation.Rate >= rateMin!.Value)
                    .WhereIf(rateMax.HasValue, e => e.BuildingEvaluation.Rate <= rateMax!.Value)
                    .WhereIf(evaluatorId != null && evaluatorId != Guid.Empty, e => e.Evaluator != null && e.Evaluator.Id == evaluatorId)
                    .WhereIf(buildingId != null && buildingId != Guid.Empty, e => e.Building != null && e.Building.Id == buildingId);
        }

        public virtual async Task<List<BuildingEvaluation>> GetListAsync(
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, rateMin, rateMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? BuildingEvaluationConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            Guid? evaluatorId = null,
            Guid? buildingId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, rateMin, rateMax, evaluatorId, buildingId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<BuildingEvaluation> ApplyFilter(
            IQueryable<BuildingEvaluation> query,
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => true)
                    .WhereIf(rateMin.HasValue, e => e.Rate >= rateMin!.Value)
                    .WhereIf(rateMax.HasValue, e => e.Rate <= rateMax!.Value);
        }
    }
}