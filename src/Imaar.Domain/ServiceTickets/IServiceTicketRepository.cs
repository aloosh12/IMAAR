using Imaar.ServiceTickets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.ServiceTickets
{
    public partial interface IServiceTicketRepository : IRepository<ServiceTicket, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? description = null,
            TicketEntityType? ticketEntityType = null,
            string? ticketEntityId = null,
            Guid? serviceTicketTypeId = null,
            Guid? ticketCreatorId = null,
            CancellationToken cancellationToken = default);
        Task<ServiceTicketWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<ServiceTicketWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? description = null,
            TicketEntityType? ticketEntityType = null,
            string? ticketEntityId = null,
            Guid? serviceTicketTypeId = null,
            Guid? ticketCreatorId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<ServiceTicket>> GetListAsync(
                    string? filterText = null,
                    string? description = null,
                    TicketEntityType? ticketEntityType = null,
                    string? ticketEntityId = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? description = null,
            TicketEntityType? ticketEntityType = null,
            string? ticketEntityId = null,
            Guid? serviceTicketTypeId = null,
            Guid? ticketCreatorId = null,
            CancellationToken cancellationToken = default);
    }
}