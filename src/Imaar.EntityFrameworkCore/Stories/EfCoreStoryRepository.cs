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

namespace Imaar.Stories
{
    public abstract class EfCoreStoryRepositoryBase : EfCoreRepository<ImaarDbContext, Story, Guid>
    {
        public EfCoreStoryRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? title = null,
            DateTime? fromTimeMin = null,
            DateTime? fromTimeMax = null,
            DateTime? expiryTimeMin = null,
            DateTime? expiryTimeMax = null,
            Guid? storyPublisherId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, title, fromTimeMin, fromTimeMax, expiryTimeMin, expiryTimeMax, storyPublisherId);

            var ids = query.Select(x => x.Story.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<StoryWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(story => new StoryWithNavigationProperties
                {
                    Story = story,
                    StoryPublisher = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == story.StoryPublisherId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<StoryWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? title = null,
            DateTime? fromTimeMin = null,
            DateTime? fromTimeMax = null,
            DateTime? expiryTimeMin = null,
            DateTime? expiryTimeMax = null,
            Guid? storyPublisherId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, title, fromTimeMin, fromTimeMax, expiryTimeMin, expiryTimeMax, storyPublisherId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? StoryConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<StoryWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from story in (await GetDbSetAsync())
                   join storyPublisher in (await GetDbContextAsync()).Set<UserProfile>() on story.StoryPublisherId equals storyPublisher.Id into userProfiles
                   from storyPublisher in userProfiles.DefaultIfEmpty()
                   select new StoryWithNavigationProperties
                   {
                       Story = story,
                       StoryPublisher = storyPublisher
                   };
        }

        protected virtual IQueryable<StoryWithNavigationProperties> ApplyFilter(
            IQueryable<StoryWithNavigationProperties> query,
            string? filterText,
            string? title = null,
            DateTime? fromTimeMin = null,
            DateTime? fromTimeMax = null,
            DateTime? expiryTimeMin = null,
            DateTime? expiryTimeMax = null,
            Guid? storyPublisherId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Story.Title!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(title), e => e.Story.Title.Contains(title))
                    .WhereIf(fromTimeMin.HasValue, e => e.Story.FromTime >= fromTimeMin!.Value)
                    .WhereIf(fromTimeMax.HasValue, e => e.Story.FromTime <= fromTimeMax!.Value)
                    .WhereIf(expiryTimeMin.HasValue, e => e.Story.ExpiryTime >= expiryTimeMin!.Value)
                    .WhereIf(expiryTimeMax.HasValue, e => e.Story.ExpiryTime <= expiryTimeMax!.Value)
                    .WhereIf(storyPublisherId != null && storyPublisherId != Guid.Empty, e => e.StoryPublisher != null && e.StoryPublisher.Id == storyPublisherId);
        }

        public virtual async Task<List<Story>> GetListAsync(
            string? filterText = null,
            string? title = null,
            DateTime? fromTimeMin = null,
            DateTime? fromTimeMax = null,
            DateTime? expiryTimeMin = null,
            DateTime? expiryTimeMax = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, title, fromTimeMin, fromTimeMax, expiryTimeMin, expiryTimeMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? StoryConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? title = null,
            DateTime? fromTimeMin = null,
            DateTime? fromTimeMax = null,
            DateTime? expiryTimeMin = null,
            DateTime? expiryTimeMax = null,
            Guid? storyPublisherId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, title, fromTimeMin, fromTimeMax, expiryTimeMin, expiryTimeMax, storyPublisherId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Story> ApplyFilter(
            IQueryable<Story> query,
            string? filterText = null,
            string? title = null,
            DateTime? fromTimeMin = null,
            DateTime? fromTimeMax = null,
            DateTime? expiryTimeMin = null,
            DateTime? expiryTimeMax = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Title!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(title), e => e.Title.Contains(title))
                    .WhereIf(fromTimeMin.HasValue, e => e.FromTime >= fromTimeMin!.Value)
                    .WhereIf(fromTimeMax.HasValue, e => e.FromTime <= fromTimeMax!.Value)
                    .WhereIf(expiryTimeMin.HasValue, e => e.ExpiryTime >= expiryTimeMin!.Value)
                    .WhereIf(expiryTimeMax.HasValue, e => e.ExpiryTime <= expiryTimeMax!.Value);
        }
    }
}