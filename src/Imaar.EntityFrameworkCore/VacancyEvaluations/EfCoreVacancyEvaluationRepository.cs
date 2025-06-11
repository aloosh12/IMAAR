using Imaar.Vacancies;
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

namespace Imaar.VacancyEvaluations
{
    public abstract class EfCoreVacancyEvaluationRepositoryBase : EfCoreRepository<ImaarDbContext, VacancyEvaluation, Guid>
    {
        public EfCoreVacancyEvaluationRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        int? rateMin = null,
            int? rateMax = null,
            Guid? userProfileId = null,
            Guid? vacancyId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, rateMin, rateMax, userProfileId, vacancyId);

            var ids = query.Select(x => x.VacancyEvaluation.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<VacancyEvaluationWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(vacancyEvaluation => new VacancyEvaluationWithNavigationProperties
                {
                    VacancyEvaluation = vacancyEvaluation,
                    UserProfile = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == vacancyEvaluation.UserProfileId),
                    Vacancy = dbContext.Set<Vacancy>().FirstOrDefault(c => c.Id == vacancyEvaluation.VacancyId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<VacancyEvaluationWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            Guid? userProfileId = null,
            Guid? vacancyId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, rateMin, rateMax, userProfileId, vacancyId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? VacancyEvaluationConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<VacancyEvaluationWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from vacancyEvaluation in (await GetDbSetAsync())
                   join userProfile in (await GetDbContextAsync()).Set<UserProfile>() on vacancyEvaluation.UserProfileId equals userProfile.Id into userProfiles
                   from userProfile in userProfiles.DefaultIfEmpty()
                   join vacancy in (await GetDbContextAsync()).Set<Vacancy>() on vacancyEvaluation.VacancyId equals vacancy.Id into vacancies
                   from vacancy in vacancies.DefaultIfEmpty()
                   select new VacancyEvaluationWithNavigationProperties
                   {
                       VacancyEvaluation = vacancyEvaluation,
                       UserProfile = userProfile,
                       Vacancy = vacancy
                   };
        }

        protected virtual IQueryable<VacancyEvaluationWithNavigationProperties> ApplyFilter(
            IQueryable<VacancyEvaluationWithNavigationProperties> query,
            string? filterText,
            int? rateMin = null,
            int? rateMax = null,
            Guid? userProfileId = null,
            Guid? vacancyId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => true)
                    .WhereIf(rateMin.HasValue, e => e.VacancyEvaluation.Rate >= rateMin!.Value)
                    .WhereIf(rateMax.HasValue, e => e.VacancyEvaluation.Rate <= rateMax!.Value)
                    .WhereIf(userProfileId != null && userProfileId != Guid.Empty, e => e.UserProfile != null && e.UserProfile.Id == userProfileId)
                    .WhereIf(vacancyId != null && vacancyId != Guid.Empty, e => e.Vacancy != null && e.Vacancy.Id == vacancyId);
        }

        public virtual async Task<List<VacancyEvaluation>> GetListAsync(
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, rateMin, rateMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? VacancyEvaluationConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            Guid? userProfileId = null,
            Guid? vacancyId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, rateMin, rateMax, userProfileId, vacancyId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<VacancyEvaluation> ApplyFilter(
            IQueryable<VacancyEvaluation> query,
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