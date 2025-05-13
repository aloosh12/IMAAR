using Imaar.Stories;
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

namespace Imaar.StoryLovers
{
    public abstract class EfCoreStoryLoverRepositoryBase : EfCoreRepository<ImaarDbContext, StoryLover, Guid>
    {
        public EfCoreStoryLoverRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        Guid? userProfileId = null,
            Guid? storyId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, userProfileId, storyId);

            var ids = query.Select(x => x.StoryLover.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<StoryLoverWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(storyLover => new StoryLoverWithNavigationProperties
                {
                    StoryLover = storyLover,
                    UserProfile = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == storyLover.UserProfileId),
                    Story = dbContext.Set<Story>().FirstOrDefault(c => c.Id == storyLover.StoryId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<StoryLoverWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            Guid? userProfileId = null,
            Guid? storyId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, userProfileId, storyId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? StoryLoverConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<StoryLoverWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from storyLover in (await GetDbSetAsync())
                   join userProfile in (await GetDbContextAsync()).Set<UserProfile>() on storyLover.UserProfileId equals userProfile.Id into userProfiles
                   from userProfile in userProfiles.DefaultIfEmpty()
                   join story in (await GetDbContextAsync()).Set<Story>() on storyLover.StoryId equals story.Id into stories
                   from story in stories.DefaultIfEmpty()
                   select new StoryLoverWithNavigationProperties
                   {
                       StoryLover = storyLover,
                       UserProfile = userProfile,
                       Story = story
                   };
        }

        protected virtual IQueryable<StoryLoverWithNavigationProperties> ApplyFilter(
            IQueryable<StoryLoverWithNavigationProperties> query,
            string? filterText,
            Guid? userProfileId = null,
            Guid? storyId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => true)
                    .WhereIf(userProfileId != null && userProfileId != Guid.Empty, e => e.UserProfile != null && e.UserProfile.Id == userProfileId)
                    .WhereIf(storyId != null && storyId != Guid.Empty, e => e.Story != null && e.Story.Id == storyId);
        }

        public virtual async Task<List<StoryLover>> GetListAsync(
            string? filterText = null,

            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? StoryLoverConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            Guid? userProfileId = null,
            Guid? storyId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, userProfileId, storyId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<StoryLover> ApplyFilter(
            IQueryable<StoryLover> query,
            string? filterText = null
)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => true)
;
        }
    }
}