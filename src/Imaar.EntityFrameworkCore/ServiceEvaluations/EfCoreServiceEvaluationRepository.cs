using Imaar.ImaarServices;
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

namespace Imaar.ServiceEvaluations
{
    public abstract class EfCoreServiceEvaluationRepositoryBase : EfCoreRepository<ImaarDbContext, ServiceEvaluation, Guid>
    {
        public EfCoreServiceEvaluationRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        int? rateMin = null,
            int? rateMax = null,
            Guid? evaluatorId = null,
            Guid? imaarServiceId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, rateMin, rateMax, evaluatorId, imaarServiceId);

            var ids = query.Select(x => x.ServiceEvaluation.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<ServiceEvaluationWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(serviceEvaluation => new ServiceEvaluationWithNavigationProperties
                {
                    ServiceEvaluation = serviceEvaluation,
                    Evaluator = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == serviceEvaluation.EvaluatorId),
                    ImaarService = dbContext.Set<ImaarService>().FirstOrDefault(c => c.Id == serviceEvaluation.ImaarServiceId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<ServiceEvaluationWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            Guid? evaluatorId = null,
            Guid? imaarServiceId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, rateMin, rateMax, evaluatorId, imaarServiceId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ServiceEvaluationConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<ServiceEvaluationWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from serviceEvaluation in (await GetDbSetAsync())
                   join evaluator in (await GetDbContextAsync()).Set<UserProfile>() on serviceEvaluation.EvaluatorId equals evaluator.Id into userProfiles
                   from evaluator in userProfiles.DefaultIfEmpty()
                   join imaarService in (await GetDbContextAsync()).Set<ImaarService>() on serviceEvaluation.ImaarServiceId equals imaarService.Id into imaarServices
                   from imaarService in imaarServices.DefaultIfEmpty()
                   select new ServiceEvaluationWithNavigationProperties
                   {
                       ServiceEvaluation = serviceEvaluation,
                       Evaluator = evaluator,
                       ImaarService = imaarService
                   };
        }

        protected virtual IQueryable<ServiceEvaluationWithNavigationProperties> ApplyFilter(
            IQueryable<ServiceEvaluationWithNavigationProperties> query,
            string? filterText,
            int? rateMin = null,
            int? rateMax = null,
            Guid? evaluatorId = null,
            Guid? imaarServiceId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => true)
                    .WhereIf(rateMin.HasValue, e => e.ServiceEvaluation.Rate >= rateMin!.Value)
                    .WhereIf(rateMax.HasValue, e => e.ServiceEvaluation.Rate <= rateMax!.Value)
                    .WhereIf(evaluatorId != null && evaluatorId != Guid.Empty, e => e.Evaluator != null && e.Evaluator.Id == evaluatorId)
                    .WhereIf(imaarServiceId != null && imaarServiceId != Guid.Empty, e => e.ImaarService != null && e.ImaarService.Id == imaarServiceId);
        }

        public virtual async Task<List<ServiceEvaluation>> GetListAsync(
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, rateMin, rateMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ServiceEvaluationConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            Guid? evaluatorId = null,
            Guid? imaarServiceId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, rateMin, rateMax, evaluatorId, imaarServiceId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<ServiceEvaluation> ApplyFilter(
            IQueryable<ServiceEvaluation> query,
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