using Imaar.Medias;
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

namespace Imaar.Medias
{
    public abstract class EfCoreMediaRepositoryBase : EfCoreRepository<ImaarDbContext, Media, Guid>
    {
        public EfCoreMediaRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? title = null,
            string? file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            MediaEntityType? sourceEntityType = null,
            string? sourceEntityId = null,
            CancellationToken cancellationToken = default)
        {

            var query = await GetQueryableAsync();

            query = ApplyFilter(query, filterText, title, file, orderMin, orderMax, isActive, sourceEntityType, sourceEntityId);

            var ids = query.Select(x => x.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<Media>> GetListAsync(
            string? filterText = null,
            string? title = null,
            string? file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            MediaEntityType? sourceEntityType = null,
            string? sourceEntityId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, title, file, orderMin, orderMax, isActive, sourceEntityType, sourceEntityId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? MediaConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? title = null,
            string? file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            MediaEntityType? sourceEntityType = null,
            string? sourceEntityId = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, title, file, orderMin, orderMax, isActive, sourceEntityType, sourceEntityId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Media> ApplyFilter(
            IQueryable<Media> query,
            string? filterText = null,
            string? title = null,
            string? file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            MediaEntityType? sourceEntityType = null,
            string? sourceEntityId = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Title!.Contains(filterText!) || e.File!.Contains(filterText!) || e.SourceEntityId!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(title), e => e.Title.Contains(title))
                    .WhereIf(!string.IsNullOrWhiteSpace(file), e => e.File.Contains(file))
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin!.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax!.Value)
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive)
                    .WhereIf(sourceEntityType.HasValue, e => e.SourceEntityType == sourceEntityType)
                    .WhereIf(!string.IsNullOrWhiteSpace(sourceEntityId), e => e.SourceEntityId.Contains(sourceEntityId));
        }
    }
}