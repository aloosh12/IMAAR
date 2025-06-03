using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.StoryTickets
{
    public partial interface IStoryTicketRepository : IRepository<StoryTicket, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? description = null,
            Guid? storyTicketTypeId = null,
            Guid? ticketCreatorId = null,
            Guid? storyAgainstId = null,
            CancellationToken cancellationToken = default);
        Task<StoryTicketWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<StoryTicketWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? description = null,
            Guid? storyTicketTypeId = null,
            Guid? ticketCreatorId = null,
            Guid? storyAgainstId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<StoryTicket>> GetListAsync(
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
            Guid? storyTicketTypeId = null,
            Guid? ticketCreatorId = null,
            Guid? storyAgainstId = null,
            CancellationToken cancellationToken = default);
    }
}