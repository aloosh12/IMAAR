using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.Tickets
{
    public abstract class TicketManagerBase : DomainService
    {
        protected ITicketRepository _ticketRepository;

        public TicketManagerBase(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public virtual async Task<Ticket> CreateAsync(
        Guid ticketTypeId, Guid ticketCreatorId, Guid ticketAgainstId, string description)
        {
            Check.NotNull(ticketTypeId, nameof(ticketTypeId));
            Check.NotNull(ticketCreatorId, nameof(ticketCreatorId));
            Check.NotNull(ticketAgainstId, nameof(ticketAgainstId));
            Check.NotNullOrWhiteSpace(description, nameof(description));

            var ticket = new Ticket(
             GuidGenerator.Create(),
             ticketTypeId, ticketCreatorId, ticketAgainstId, description
             );

            return await _ticketRepository.InsertAsync(ticket);
        }

        public virtual async Task<Ticket> UpdateAsync(
            Guid id,
            Guid ticketTypeId, Guid ticketCreatorId, Guid ticketAgainstId, string description, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(ticketTypeId, nameof(ticketTypeId));
            Check.NotNull(ticketCreatorId, nameof(ticketCreatorId));
            Check.NotNull(ticketAgainstId, nameof(ticketAgainstId));
            Check.NotNullOrWhiteSpace(description, nameof(description));

            var ticket = await _ticketRepository.GetAsync(id);

            ticket.TicketTypeId = ticketTypeId;
            ticket.TicketCreatorId = ticketCreatorId;
            ticket.TicketAgainstId = ticketAgainstId;
            ticket.Description = description;

            ticket.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _ticketRepository.UpdateAsync(ticket);
        }

    }
}