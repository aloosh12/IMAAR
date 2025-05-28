using Imaar.ServiceTickets;
using Imaar.UserProfiles;
using Imaar.ServiceTicketTypes;
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

namespace Imaar.ServiceTickets
{
    public abstract class EfCoreServiceTicketRepositoryBase : EfCoreRepository<ImaarDbContext, ServiceTicket, Guid>
    {
        public EfCoreServiceTicketRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? description = null,
            TicketEntityType? ticketEntityType = null,
            string? ticketEntityId = null,
            Guid? serviceTicketTypeId = null,
            Guid? ticketCreatorId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, description, ticketEntityType, ticketEntityId, serviceTicketTypeId, ticketCreatorId);

            var ids = query.Select(x => x.ServiceTicket.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<ServiceTicketWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(serviceTicket => new ServiceTicketWithNavigationProperties
                {
                    ServiceTicket = serviceTicket,
                    ServiceTicketType = dbContext.Set<ServiceTicketType>().FirstOrDefault(c => c.Id == serviceTicket.ServiceTicketTypeId),
                    TicketCreator = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == serviceTicket.TicketCreatorId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<ServiceTicketWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? description = null,
            TicketEntityType? ticketEntityType = null,
            string? ticketEntityId = null,
            Guid? serviceTicketTypeId = null,
            Guid? ticketCreatorId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, description, ticketEntityType, ticketEntityId, serviceTicketTypeId, ticketCreatorId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ServiceTicketConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<ServiceTicketWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from serviceTicket in (await GetDbSetAsync())
                   join serviceTicketType in (await GetDbContextAsync()).Set<ServiceTicketType>() on serviceTicket.ServiceTicketTypeId equals serviceTicketType.Id into serviceTicketTypes
                   from serviceTicketType in serviceTicketTypes.DefaultIfEmpty()
                   join ticketCreator in (await GetDbContextAsync()).Set<UserProfile>() on serviceTicket.TicketCreatorId equals ticketCreator.Id into userProfiles
                   from ticketCreator in userProfiles.DefaultIfEmpty()
                   select new ServiceTicketWithNavigationProperties
                   {
                       ServiceTicket = serviceTicket,
                       ServiceTicketType = serviceTicketType,
                       TicketCreator = ticketCreator
                   };
        }

        protected virtual IQueryable<ServiceTicketWithNavigationProperties> ApplyFilter(
            IQueryable<ServiceTicketWithNavigationProperties> query,
            string? filterText,
            string? description = null,
            TicketEntityType? ticketEntityType = null,
            string? ticketEntityId = null,
            Guid? serviceTicketTypeId = null,
            Guid? ticketCreatorId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.ServiceTicket.Description!.Contains(filterText!) || e.ServiceTicket.TicketEntityId!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.ServiceTicket.Description.Contains(description))
                    .WhereIf(ticketEntityType.HasValue, e => e.ServiceTicket.TicketEntityType == ticketEntityType)
                    .WhereIf(!string.IsNullOrWhiteSpace(ticketEntityId), e => e.ServiceTicket.TicketEntityId.Contains(ticketEntityId))
                    .WhereIf(serviceTicketTypeId != null && serviceTicketTypeId != Guid.Empty, e => e.ServiceTicketType != null && e.ServiceTicketType.Id == serviceTicketTypeId)
                    .WhereIf(ticketCreatorId != null && ticketCreatorId != Guid.Empty, e => e.TicketCreator != null && e.TicketCreator.Id == ticketCreatorId);
        }

        public virtual async Task<List<ServiceTicket>> GetListAsync(
            string? filterText = null,
            string? description = null,
            TicketEntityType? ticketEntityType = null,
            string? ticketEntityId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, description, ticketEntityType, ticketEntityId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ServiceTicketConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? description = null,
            TicketEntityType? ticketEntityType = null,
            string? ticketEntityId = null,
            Guid? serviceTicketTypeId = null,
            Guid? ticketCreatorId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, description, ticketEntityType, ticketEntityId, serviceTicketTypeId, ticketCreatorId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<ServiceTicket> ApplyFilter(
            IQueryable<ServiceTicket> query,
            string? filterText = null,
            string? description = null,
            TicketEntityType? ticketEntityType = null,
            string? ticketEntityId = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Description!.Contains(filterText!) || e.TicketEntityId!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description))
                    .WhereIf(ticketEntityType.HasValue, e => e.TicketEntityType == ticketEntityType)
                    .WhereIf(!string.IsNullOrWhiteSpace(ticketEntityId), e => e.TicketEntityId.Contains(ticketEntityId));
        }
    }
}