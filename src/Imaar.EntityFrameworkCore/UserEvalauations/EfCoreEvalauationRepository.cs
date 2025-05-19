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

namespace Imaar.UserEvalauations
{
    public abstract class EfCoreUserEvalauationRepositoryBase : EfCoreRepository<ImaarDbContext, UserEvalauation, Guid>
    {
        public EfCoreUserEvalauationRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
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

            var ids = query.Select(x => x.UserEvalauation.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<UserEvalauationWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(userEvalauation => new UserEvalauationWithNavigationProperties
                {
                    UserEvalauation = userEvalauation,
                    Evaluatord = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == userEvalauation.Evaluatord),
                    EvaluatedPerson = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == userEvalauation.EvaluatedPersonId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<UserEvalauationWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserEvalauationConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<UserEvalauationWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from userEvalauation in (await GetDbSetAsync())
                   join evaluatord in (await GetDbContextAsync()).Set<UserProfile>() on userEvalauation.Evaluatord equals evaluatord.Id into userProfiles
                   from evaluatord in userProfiles.DefaultIfEmpty()
                   join evaluatedPerson in (await GetDbContextAsync()).Set<UserProfile>() on userEvalauation.EvaluatedPersonId equals evaluatedPerson.Id into userProfiles1
                   from evaluatedPerson in userProfiles1.DefaultIfEmpty()
                   select new UserEvalauationWithNavigationProperties
                   {
                       UserEvalauation = userEvalauation,
                       Evaluatord = evaluatord,
                       EvaluatedPerson = evaluatedPerson
                   };
        }

        protected virtual IQueryable<UserEvalauationWithNavigationProperties> ApplyFilter(
            IQueryable<UserEvalauationWithNavigationProperties> query,
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
                    .WhereIf(speedOfCompletionMin.HasValue, e => e.UserEvalauation.SpeedOfCompletion >= speedOfCompletionMin!.Value)
                    .WhereIf(speedOfCompletionMax.HasValue, e => e.UserEvalauation.SpeedOfCompletion <= speedOfCompletionMax!.Value)
                    .WhereIf(dealingMin.HasValue, e => e.UserEvalauation.Dealing >= dealingMin!.Value)
                    .WhereIf(dealingMax.HasValue, e => e.UserEvalauation.Dealing <= dealingMax!.Value)
                    .WhereIf(cleanlinessMin.HasValue, e => e.UserEvalauation.Cleanliness >= cleanlinessMin!.Value)
                    .WhereIf(cleanlinessMax.HasValue, e => e.UserEvalauation.Cleanliness <= cleanlinessMax!.Value)
                    .WhereIf(perfectionMin.HasValue, e => e.UserEvalauation.Perfection >= perfectionMin!.Value)
                    .WhereIf(perfectionMax.HasValue, e => e.UserEvalauation.Perfection <= perfectionMax!.Value)
                    .WhereIf(priceMin.HasValue, e => e.UserEvalauation.Price >= priceMin!.Value)
                    .WhereIf(priceMax.HasValue, e => e.UserEvalauation.Price <= priceMax!.Value)
                    .WhereIf(evaluatord != null && evaluatord != Guid.Empty, e => e.Evaluatord != null && e.Evaluatord.Id == evaluatord)
                    .WhereIf(evaluatedPersonId != null && evaluatedPersonId != Guid.Empty, e => e.EvaluatedPerson != null && e.EvaluatedPerson.Id == evaluatedPersonId);
        }

        public virtual async Task<List<UserEvalauation>> GetListAsync(
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
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserEvalauationConsts.GetDefaultSorting(false) : sorting);
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

        protected virtual IQueryable<UserEvalauation> ApplyFilter(
            IQueryable<UserEvalauation> query,
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