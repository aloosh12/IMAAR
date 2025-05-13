using Imaar.UserProfiles;
using Imaar.TicketTypes;
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

namespace Imaar.Tickets
{
    public abstract class EfCoreTicketRepositoryBase : EfCoreRepository<ImaarDbContext, Ticket, Guid>
    {
        public EfCoreTicketRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? description = null,
            Guid? ticketTypeId = null,
            Guid? ticketCreatorId = null,
            Guid? ticketAgainstId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, description, ticketTypeId, ticketCreatorId, ticketAgainstId);

            var ids = query.Select(x => x.Ticket.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<TicketWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(ticket => new TicketWithNavigationProperties
                {
                    Ticket = ticket,
                    TicketType = dbContext.Set<TicketType>().FirstOrDefault(c => c.Id == ticket.TicketTypeId),
                    TicketCreator = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == ticket.TicketCreatorId),
                    TicketAgainst = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == ticket.TicketAgainstId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<TicketWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? description = null,
            Guid? ticketTypeId = null,
            Guid? ticketCreatorId = null,
            Guid? ticketAgainstId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, description, ticketTypeId, ticketCreatorId, ticketAgainstId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? TicketConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<TicketWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from ticket in (await GetDbSetAsync())
                   join ticketType in (await GetDbContextAsync()).Set<TicketType>() on ticket.TicketTypeId equals ticketType.Id into ticketTypes
                   from ticketType in ticketTypes.DefaultIfEmpty()
                   join ticketCreator in (await GetDbContextAsync()).Set<UserProfile>() on ticket.TicketCreatorId equals ticketCreator.Id into userProfiles
                   from ticketCreator in userProfiles.DefaultIfEmpty()
                   join ticketAgainst in (await GetDbContextAsync()).Set<UserProfile>() on ticket.TicketAgainstId equals ticketAgainst.Id into userProfiles1
                   from ticketAgainst in userProfiles1.DefaultIfEmpty()
                   select new TicketWithNavigationProperties
                   {
                       Ticket = ticket,
                       TicketType = ticketType,
                       TicketCreator = ticketCreator,
                       TicketAgainst = ticketAgainst
                   };
        }

        protected virtual IQueryable<TicketWithNavigationProperties> ApplyFilter(
            IQueryable<TicketWithNavigationProperties> query,
            string? filterText,
            string? description = null,
            Guid? ticketTypeId = null,
            Guid? ticketCreatorId = null,
            Guid? ticketAgainstId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Ticket.Description!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Ticket.Description.Contains(description))
                    .WhereIf(ticketTypeId != null && ticketTypeId != Guid.Empty, e => e.TicketType != null && e.TicketType.Id == ticketTypeId)
                    .WhereIf(ticketCreatorId != null && ticketCreatorId != Guid.Empty, e => e.TicketCreator != null && e.TicketCreator.Id == ticketCreatorId)
                    .WhereIf(ticketAgainstId != null && ticketAgainstId != Guid.Empty, e => e.TicketAgainst != null && e.TicketAgainst.Id == ticketAgainstId);
        }

        public virtual async Task<List<Ticket>> GetListAsync(
            string? filterText = null,
            string? description = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, description);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? TicketConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? description = null,
            Guid? ticketTypeId = null,
            Guid? ticketCreatorId = null,
            Guid? ticketAgainstId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, description, ticketTypeId, ticketCreatorId, ticketAgainstId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Ticket> ApplyFilter(
            IQueryable<Ticket> query,
            string? filterText = null,
            string? description = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Description!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description));
        }
    }
}