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

namespace Imaar.Evalauations
{
    public abstract class EfCoreEvalauationRepositoryBase : EfCoreRepository<ImaarDbContext, Evalauation, Guid>
    {
        public EfCoreEvalauationRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        int? speedOfCompletionMin = null,
            int? speedOfCompletionMax = null,
            int? dealingMin = null,
            int? dealingMax = null,
            int? cleanlinessMin = null,
            int? cleanlinessMax = null,
            int? perfectionMin = null,
            int? perfectionMax = null,
            int? priceMin = null,
            int? priceMax = null,
            Guid? evaluatord = null,
            Guid? evaluatedPersonId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, speedOfCompletionMin, speedOfCompletionMax, dealingMin, dealingMax, cleanlinessMin, cleanlinessMax, perfectionMin, perfectionMax, priceMin, priceMax, evaluatord, evaluatedPersonId);

            var ids = query.Select(x => x.Evalauation.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<EvalauationWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(evalauation => new EvalauationWithNavigationProperties
                {
                    Evalauation = evalauation,
                    Evaluatord = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == evalauation.Evaluatord),
                    EvaluatedPerson = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == evalauation.EvaluatedPersonId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<EvalauationWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            int? speedOfCompletionMin = null,
            int? speedOfCompletionMax = null,
            int? dealingMin = null,
            int? dealingMax = null,
            int? cleanlinessMin = null,
            int? cleanlinessMax = null,
            int? perfectionMin = null,
            int? perfectionMax = null,
            int? priceMin = null,
            int? priceMax = null,
            Guid? evaluatord = null,
            Guid? evaluatedPersonId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, speedOfCompletionMin, speedOfCompletionMax, dealingMin, dealingMax, cleanlinessMin, cleanlinessMax, perfectionMin, perfectionMax, priceMin, priceMax, evaluatord, evaluatedPersonId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? EvalauationConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<EvalauationWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from evalauation in (await GetDbSetAsync())
                   join evaluatord in (await GetDbContextAsync()).Set<UserProfile>() on evalauation.Evaluatord equals evaluatord.Id into userProfiles
                   from evaluatord in userProfiles.DefaultIfEmpty()
                   join evaluatedPerson in (await GetDbContextAsync()).Set<UserProfile>() on evalauation.EvaluatedPersonId equals evaluatedPerson.Id into userProfiles1
                   from evaluatedPerson in userProfiles1.DefaultIfEmpty()
                   select new EvalauationWithNavigationProperties
                   {
                       Evalauation = evalauation,
                       Evaluatord = evaluatord,
                       EvaluatedPerson = evaluatedPerson
                   };
        }

        protected virtual IQueryable<EvalauationWithNavigationProperties> ApplyFilter(
            IQueryable<EvalauationWithNavigationProperties> query,
            string? filterText,
            int? speedOfCompletionMin = null,
            int? speedOfCompletionMax = null,
            int? dealingMin = null,
            int? dealingMax = null,
            int? cleanlinessMin = null,
            int? cleanlinessMax = null,
            int? perfectionMin = null,
            int? perfectionMax = null,
            int? priceMin = null,
            int? priceMax = null,
            Guid? evaluatord = null,
            Guid? evaluatedPersonId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => true)
                    .WhereIf(speedOfCompletionMin.HasValue, e => e.Evalauation.SpeedOfCompletion >= speedOfCompletionMin!.Value)
                    .WhereIf(speedOfCompletionMax.HasValue, e => e.Evalauation.SpeedOfCompletion <= speedOfCompletionMax!.Value)
                    .WhereIf(dealingMin.HasValue, e => e.Evalauation.Dealing >= dealingMin!.Value)
                    .WhereIf(dealingMax.HasValue, e => e.Evalauation.Dealing <= dealingMax!.Value)
                    .WhereIf(cleanlinessMin.HasValue, e => e.Evalauation.Cleanliness >= cleanlinessMin!.Value)
                    .WhereIf(cleanlinessMax.HasValue, e => e.Evalauation.Cleanliness <= cleanlinessMax!.Value)
                    .WhereIf(perfectionMin.HasValue, e => e.Evalauation.Perfection >= perfectionMin!.Value)
                    .WhereIf(perfectionMax.HasValue, e => e.Evalauation.Perfection <= perfectionMax!.Value)
                    .WhereIf(priceMin.HasValue, e => e.Evalauation.Price >= priceMin!.Value)
                    .WhereIf(priceMax.HasValue, e => e.Evalauation.Price <= priceMax!.Value)
                    .WhereIf(evaluatord != null && evaluatord != Guid.Empty, e => e.Evaluatord != null && e.Evaluatord.Id == evaluatord)
                    .WhereIf(evaluatedPersonId != null && evaluatedPersonId != Guid.Empty, e => e.EvaluatedPerson != null && e.EvaluatedPerson.Id == evaluatedPersonId);
        }

        public virtual async Task<List<Evalauation>> GetListAsync(
            string? filterText = null,
            int? speedOfCompletionMin = null,
            int? speedOfCompletionMax = null,
            int? dealingMin = null,
            int? dealingMax = null,
            int? cleanlinessMin = null,
            int? cleanlinessMax = null,
            int? perfectionMin = null,
            int? perfectionMax = null,
            int? priceMin = null,
            int? priceMax = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, speedOfCompletionMin, speedOfCompletionMax, dealingMin, dealingMax, cleanlinessMin, cleanlinessMax, perfectionMin, perfectionMax, priceMin, priceMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? EvalauationConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            int? speedOfCompletionMin = null,
            int? speedOfCompletionMax = null,
            int? dealingMin = null,
            int? dealingMax = null,
            int? cleanlinessMin = null,
            int? cleanlinessMax = null,
            int? perfectionMin = null,
            int? perfectionMax = null,
            int? priceMin = null,
            int? priceMax = null,
            Guid? evaluatord = null,
            Guid? evaluatedPersonId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, speedOfCompletionMin, speedOfCompletionMax, dealingMin, dealingMax, cleanlinessMin, cleanlinessMax, perfectionMin, perfectionMax, priceMin, priceMax, evaluatord, evaluatedPersonId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Evalauation> ApplyFilter(
            IQueryable<Evalauation> query,
            string? filterText = null,
            int? speedOfCompletionMin = null,
            int? speedOfCompletionMax = null,
            int? dealingMin = null,
            int? dealingMax = null,
            int? cleanlinessMin = null,
            int? cleanlinessMax = null,
            int? perfectionMin = null,
            int? perfectionMax = null,
            int? priceMin = null,
            int? priceMax = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => true)
                    .WhereIf(speedOfCompletionMin.HasValue, e => e.SpeedOfCompletion >= speedOfCompletionMin!.Value)
                    .WhereIf(speedOfCompletionMax.HasValue, e => e.SpeedOfCompletion <= speedOfCompletionMax!.Value)
                    .WhereIf(dealingMin.HasValue, e => e.Dealing >= dealingMin!.Value)
                    .WhereIf(dealingMax.HasValue, e => e.Dealing <= dealingMax!.Value)
                    .WhereIf(cleanlinessMin.HasValue, e => e.Cleanliness >= cleanlinessMin!.Value)
                    .WhereIf(cleanlinessMax.HasValue, e => e.Cleanliness <= cleanlinessMax!.Value)
                    .WhereIf(perfectionMin.HasValue, e => e.Perfection >= perfectionMin!.Value)
                    .WhereIf(perfectionMax.HasValue, e => e.Perfection <= perfectionMax!.Value)
                    .WhereIf(priceMin.HasValue, e => e.Price >= priceMin!.Value)
                    .WhereIf(priceMax.HasValue, e => e.Price <= priceMax!.Value);
        }
    }
}