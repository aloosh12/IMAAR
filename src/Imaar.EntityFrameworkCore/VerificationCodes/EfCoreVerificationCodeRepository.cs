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

namespace Imaar.VerificationCodes
{
    public abstract class EfCoreVerificationCodeRepositoryBase : EfCoreRepository<ImaarDbContext, VerificationCode, Guid>
    {
        public EfCoreVerificationCodeRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? phoneNumber = null,
            int? securityCodeMin = null,
            int? securityCodeMax = null,
            bool? isFinish = null,
            CancellationToken cancellationToken = default)
        {

            var query = await GetQueryableAsync();

            query = ApplyFilter(query, filterText, phoneNumber, securityCodeMin, securityCodeMax, isFinish);

            var ids = query.Select(x => x.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<VerificationCode>> GetListAsync(
            string? filterText = null,
            string? phoneNumber = null,
            int? securityCodeMin = null,
            int? securityCodeMax = null,
            bool? isFinish = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, phoneNumber, securityCodeMin, securityCodeMax, isFinish);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? VerificationCodeConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? phoneNumber = null,
            int? securityCodeMin = null,
            int? securityCodeMax = null,
            bool? isFinish = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, phoneNumber, securityCodeMin, securityCodeMax, isFinish);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<VerificationCode> ApplyFilter(
            IQueryable<VerificationCode> query,
            string? filterText = null,
            string? phoneNumber = null,
            int? securityCodeMin = null,
            int? securityCodeMax = null,
            bool? isFinish = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.PhoneNumber!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(phoneNumber), e => e.PhoneNumber.Contains(phoneNumber))
                    .WhereIf(securityCodeMin.HasValue, e => e.SecurityCode >= securityCodeMin!.Value)
                    .WhereIf(securityCodeMax.HasValue, e => e.SecurityCode <= securityCodeMax!.Value)
                    .WhereIf(isFinish.HasValue, e => e.IsFinish == isFinish);
        }
    }
}