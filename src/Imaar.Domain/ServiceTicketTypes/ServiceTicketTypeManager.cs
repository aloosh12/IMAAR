using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.ServiceTicketTypes
{
    public abstract class ServiceTicketTypeManagerBase : DomainService
    {
        protected IServiceTicketTypeRepository _serviceTicketTypeRepository;

        public ServiceTicketTypeManagerBase(IServiceTicketTypeRepository serviceTicketTypeRepository)
        {
            _serviceTicketTypeRepository = serviceTicketTypeRepository;
        }

        public virtual async Task<ServiceTicketType> CreateAsync(
        string title, bool isActive, int? order = null)
        {
            Check.NotNullOrWhiteSpace(title, nameof(title));

            var serviceTicketType = new ServiceTicketType(
             GuidGenerator.Create(),
             title, isActive, order
             );

            return await _serviceTicketTypeRepository.InsertAsync(serviceTicketType);
        }

        public virtual async Task<ServiceTicketType> UpdateAsync(
            Guid id,
            string title, bool isActive, int? order = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(title, nameof(title));

            var serviceTicketType = await _serviceTicketTypeRepository.GetAsync(id);

            serviceTicketType.Title = title;
            serviceTicketType.IsActive = isActive;
            serviceTicketType.Order = order;

            serviceTicketType.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _serviceTicketTypeRepository.UpdateAsync(serviceTicketType);
        }

    }
}