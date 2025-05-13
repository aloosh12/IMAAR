using Imaar.Stories;
using Imaar.Vacancies;
using Imaar.ImaarServices;
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
            Guid? imaarServiceId = null,
            Guid? vacancyId = null,
            Guid? storyId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, title, file, orderMin, orderMax, isActive, imaarServiceId, vacancyId, storyId);

            var ids = query.Select(x => x.Media.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<MediaWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(media => new MediaWithNavigationProperties
                {
                    Media = media,
                    ImaarService = dbContext.Set<ImaarService>().FirstOrDefault(c => c.Id == media.ImaarServiceId),
                    Vacancy = dbContext.Set<Vacancy>().FirstOrDefault(c => c.Id == media.VacancyId),
                    Story = dbContext.Set<Story>().FirstOrDefault(c => c.Id == media.StoryId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<MediaWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? title = null,
            string? file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? imaarServiceId = null,
            Guid? vacancyId = null,
            Guid? storyId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, title, file, orderMin, orderMax, isActive, imaarServiceId, vacancyId, storyId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? MediaConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<MediaWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from media in (await GetDbSetAsync())
                   join imaarService in (await GetDbContextAsync()).Set<ImaarService>() on media.ImaarServiceId equals imaarService.Id into imaarServices
                   from imaarService in imaarServices.DefaultIfEmpty()
                   join vacancy in (await GetDbContextAsync()).Set<Vacancy>() on media.VacancyId equals vacancy.Id into vacancies
                   from vacancy in vacancies.DefaultIfEmpty()
                   join story in (await GetDbContextAsync()).Set<Story>() on media.StoryId equals story.Id into stories
                   from story in stories.DefaultIfEmpty()
                   select new MediaWithNavigationProperties
                   {
                       Media = media,
                       ImaarService = imaarService,
                       Vacancy = vacancy,
                       Story = story
                   };
        }

        protected virtual IQueryable<MediaWithNavigationProperties> ApplyFilter(
            IQueryable<MediaWithNavigationProperties> query,
            string? filterText,
            string? title = null,
            string? file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? imaarServiceId = null,
            Guid? vacancyId = null,
            Guid? storyId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Media.Title!.Contains(filterText!) || e.Media.File!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(title), e => e.Media.Title.Contains(title))
                    .WhereIf(!string.IsNullOrWhiteSpace(file), e => e.Media.File.Contains(file))
                    .WhereIf(orderMin.HasValue, e => e.Media.Order >= orderMin!.Value)
                    .WhereIf(orderMax.HasValue, e => e.Media.Order <= orderMax!.Value)
                    .WhereIf(isActive.HasValue, e => e.Media.IsActive == isActive)
                    .WhereIf(imaarServiceId != null && imaarServiceId != Guid.Empty, e => e.ImaarService != null && e.ImaarService.Id == imaarServiceId)
                    .WhereIf(vacancyId != null && vacancyId != Guid.Empty, e => e.Vacancy != null && e.Vacancy.Id == vacancyId)
                    .WhereIf(storyId != null && storyId != Guid.Empty, e => e.Story != null && e.Story.Id == storyId);
        }

        public virtual async Task<List<Media>> GetListAsync(
            string? filterText = null,
            string? title = null,
            string? file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, title, file, orderMin, orderMax, isActive);
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
            Guid? imaarServiceId = null,
            Guid? vacancyId = null,
            Guid? storyId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, title, file, orderMin, orderMax, isActive, imaarServiceId, vacancyId, storyId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Media> ApplyFilter(
            IQueryable<Media> query,
            string? filterText = null,
            string? title = null,
            string? file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Title!.Contains(filterText!) || e.File!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(title), e => e.Title.Contains(title))
                    .WhereIf(!string.IsNullOrWhiteSpace(file), e => e.File.Contains(file))
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin!.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax!.Value)
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive);
        }
    }
}