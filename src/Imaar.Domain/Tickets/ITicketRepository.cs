using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.Tickets
{
    public partial interface ITicketRepository : IRepository<Ticket, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? description = null,
            Guid? ticketTypeId = null,
            Guid? ticketCreatorId = null,
            Guid? ticketAgainstId = null,
            CancellationToken cancellationToken = default);
        Task<TicketWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<TicketWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? description = null,
            Guid? ticketTypeId = null,
            Guid? ticketCreatorId = null,
            Guid? ticketAgainstId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<Ticket>> GetListAsync(
                    string? filterText = null,
                    string? description = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? description = null,
            Guid? ticketTypeId = null,
            Guid? ticketCreatorId = null,
            Guid? ticketAgainstId = null,
            CancellationToken cancellationToken = default);
    }
}