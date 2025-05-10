using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.TicketTypes
{
    public abstract class TicketTypeManagerBase : DomainService
    {
        protected ITicketTypeRepository _ticketTypeRepository;

        public TicketTypeManagerBase(ITicketTypeRepository ticketTypeRepository)
        {
            _ticketTypeRepository = ticketTypeRepository;
        }

        public virtual async Task<TicketType> CreateAsync(
        string title, int order, bool isActive)
        {
            Check.NotNullOrWhiteSpace(title, nameof(title));

            var ticketType = new TicketType(
             GuidGenerator.Create(),
             title, order, isActive
             );

            return await _ticketTypeRepository.InsertAsync(ticketType);
        }

        public virtual async Task<TicketType> UpdateAsync(
            Guid id,
            string title, int order, bool isActive, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(title, nameof(title));

            var ticketType = await _ticketTypeRepository.GetAsync(id);

            ticketType.Title = title;
            ticketType.Order = order;
            ticketType.IsActive = isActive;

            ticketType.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _ticketTypeRepository.UpdateAsync(ticketType);
        }

    }
}