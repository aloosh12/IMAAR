using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.StoryTickets
{
    public abstract class StoryTicketManagerBase : DomainService
    {
        protected IStoryTicketRepository _storyTicketRepository;

        public StoryTicketManagerBase(IStoryTicketRepository storyTicketRepository)
        {
            _storyTicketRepository = storyTicketRepository;
        }

        public virtual async Task<StoryTicket> CreateAsync(
        Guid storyTicketTypeId, Guid ticketCreatorId, Guid storyAgainstId, string description)
        {
            Check.NotNull(storyTicketTypeId, nameof(storyTicketTypeId));
            Check.NotNull(ticketCreatorId, nameof(ticketCreatorId));
            Check.NotNull(storyAgainstId, nameof(storyAgainstId));
            Check.NotNullOrWhiteSpace(description, nameof(description));

            var storyTicket = new StoryTicket(
             GuidGenerator.Create(),
             storyTicketTypeId, ticketCreatorId, storyAgainstId, description
             );

            return await _storyTicketRepository.InsertAsync(storyTicket);
        }

        public virtual async Task<StoryTicket> UpdateAsync(
            Guid id,
            Guid storyTicketTypeId, Guid ticketCreatorId, Guid storyAgainstId, string description, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(storyTicketTypeId, nameof(storyTicketTypeId));
            Check.NotNull(ticketCreatorId, nameof(ticketCreatorId));
            Check.NotNull(storyAgainstId, nameof(storyAgainstId));
            Check.NotNullOrWhiteSpace(description, nameof(description));

            var storyTicket = await _storyTicketRepository.GetAsync(id);

            storyTicket.StoryTicketTypeId = storyTicketTypeId;
            storyTicket.TicketCreatorId = ticketCreatorId;
            storyTicket.StoryAgainstId = storyAgainstId;
            storyTicket.Description = description;

            storyTicket.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _storyTicketRepository.UpdateAsync(storyTicket);
        }

    }
}