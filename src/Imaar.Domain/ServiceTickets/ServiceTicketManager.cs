using Imaar.ServiceTickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.ServiceTickets
{
    public abstract class ServiceTicketManagerBase : DomainService
    {
        protected IServiceTicketRepository _serviceTicketRepository;

        public ServiceTicketManagerBase(IServiceTicketRepository serviceTicketRepository)
        {
            _serviceTicketRepository = serviceTicketRepository;
        }

        public virtual async Task<ServiceTicket> CreateAsync(
        Guid serviceTicketTypeId, Guid ticketCreatorId, string description, TicketEntityType ticketEntityType, string ticketEntityId)
        {
            Check.NotNull(serviceTicketTypeId, nameof(serviceTicketTypeId));
            Check.NotNull(ticketCreatorId, nameof(ticketCreatorId));
            Check.NotNullOrWhiteSpace(description, nameof(description));
            Check.NotNull(ticketEntityType, nameof(ticketEntityType));
            Check.NotNullOrWhiteSpace(ticketEntityId, nameof(ticketEntityId));

            var serviceTicket = new ServiceTicket(
             GuidGenerator.Create(),
             serviceTicketTypeId, ticketCreatorId, description, ticketEntityType, ticketEntityId
             );

            return await _serviceTicketRepository.InsertAsync(serviceTicket);
        }

        public virtual async Task<ServiceTicket> UpdateAsync(
            Guid id,
            Guid serviceTicketTypeId, Guid ticketCreatorId, string description, TicketEntityType ticketEntityType, string ticketEntityId, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(serviceTicketTypeId, nameof(serviceTicketTypeId));
            Check.NotNull(ticketCreatorId, nameof(ticketCreatorId));
            Check.NotNullOrWhiteSpace(description, nameof(description));
            Check.NotNull(ticketEntityType, nameof(ticketEntityType));
            Check.NotNullOrWhiteSpace(ticketEntityId, nameof(ticketEntityId));

            var serviceTicket = await _serviceTicketRepository.GetAsync(id);

            serviceTicket.ServiceTicketTypeId = serviceTicketTypeId;
            serviceTicket.TicketCreatorId = ticketCreatorId;
            serviceTicket.Description = description;
            serviceTicket.TicketEntityType = ticketEntityType;
            serviceTicket.TicketEntityId = ticketEntityId;

            serviceTicket.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _serviceTicketRepository.UpdateAsync(serviceTicket);
        }

    }
}