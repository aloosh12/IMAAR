using Imaar.Stories;
using Imaar.UserProfiles;
using Imaar.StoryTicketTypes;
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

namespace Imaar.StoryTickets
{
    public abstract class EfCoreStoryTicketRepositoryBase : EfCoreRepository<ImaarDbContext, StoryTicket, Guid>
    {
        public EfCoreStoryTicketRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? description = null,
            Guid? storyTicketTypeId = null,
            Guid? ticketCreatorId = null,
            Guid? storyAgainstId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, description, storyTicketTypeId, ticketCreatorId, storyAgainstId);

            var ids = query.Select(x => x.StoryTicket.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<StoryTicketWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(storyTicket => new StoryTicketWithNavigationProperties
                {
                    StoryTicket = storyTicket,
                    StoryTicketType = dbContext.Set<StoryTicketType>().FirstOrDefault(c => c.Id == storyTicket.StoryTicketTypeId),
                    TicketCreator = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == storyTicket.TicketCreatorId),
                    StoryAgainst = dbContext.Set<Story>().FirstOrDefault(c => c.Id == storyTicket.StoryAgainstId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<StoryTicketWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? description = null,
            Guid? storyTicketTypeId = null,
            Guid? ticketCreatorId = null,
            Guid? storyAgainstId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, description, storyTicketTypeId, ticketCreatorId, storyAgainstId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? StoryTicketConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<StoryTicketWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from storyTicket in (await GetDbSetAsync())
                   join storyTicketType in (await GetDbContextAsync()).Set<StoryTicketType>() on storyTicket.StoryTicketTypeId equals storyTicketType.Id into storyTicketTypes
                   from storyTicketType in storyTicketTypes.DefaultIfEmpty()
                   join ticketCreator in (await GetDbContextAsync()).Set<UserProfile>() on storyTicket.TicketCreatorId equals ticketCreator.Id into userProfiles
                   from ticketCreator in userProfiles.DefaultIfEmpty()
                   join storyAgainst in (await GetDbContextAsync()).Set<Story>() on storyTicket.StoryAgainstId equals storyAgainst.Id into stories
                   from storyAgainst in stories.DefaultIfEmpty()
                   select new StoryTicketWithNavigationProperties
                   {
                       StoryTicket = storyTicket,
                       StoryTicketType = storyTicketType,
                       TicketCreator = ticketCreator,
                       StoryAgainst = storyAgainst
                   };
        }

        protected virtual IQueryable<StoryTicketWithNavigationProperties> ApplyFilter(
            IQueryable<StoryTicketWithNavigationProperties> query,
            string? filterText,
            string? description = null,
            Guid? storyTicketTypeId = null,
            Guid? ticketCreatorId = null,
            Guid? storyAgainstId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.StoryTicket.Description!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.StoryTicket.Description.Contains(description))
                    .WhereIf(storyTicketTypeId != null && storyTicketTypeId != Guid.Empty, e => e.StoryTicketType != null && e.StoryTicketType.Id == storyTicketTypeId)
                    .WhereIf(ticketCreatorId != null && ticketCreatorId != Guid.Empty, e => e.TicketCreator != null && e.TicketCreator.Id == ticketCreatorId)
                    .WhereIf(storyAgainstId != null && storyAgainstId != Guid.Empty, e => e.StoryAgainst != null && e.StoryAgainst.Id == storyAgainstId);
        }

        public virtual async Task<List<StoryTicket>> GetListAsync(
            string? filterText = null,
            string? description = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, description);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? StoryTicketConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? description = null,
            Guid? storyTicketTypeId = null,
            Guid? ticketCreatorId = null,
            Guid? storyAgainstId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, description, storyTicketTypeId, ticketCreatorId, storyAgainstId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<StoryTicket> ApplyFilter(
            IQueryable<StoryTicket> query,
            string? filterText = null,
            string? description = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Description!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description));
        }
    }
}