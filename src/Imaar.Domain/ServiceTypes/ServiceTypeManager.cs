using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Imaar.ServiceTypes
{
    public abstract class ServiceTypeManagerBase : DomainService
    {
        protected IServiceTypeRepository _serviceTypeRepository;

        public ServiceTypeManagerBase(IServiceTypeRepository serviceTypeRepository)
        {
            _serviceTypeRepository = serviceTypeRepository;
        }

        public virtual async Task<ServiceType> CreateAsync(
        Guid categoryId, string title, int order, bool isActive, string? icon = null)
        {
            Check.NotNullOrWhiteSpace(title, nameof(title));

            var serviceType = new ServiceType(
             GuidGenerator.Create(),
             categoryId, title, order, isActive, icon
             );

            return await _serviceTypeRepository.InsertAsync(serviceType);
        }

        public virtual async Task<ServiceType> UpdateAsync(
            Guid id,
            Guid categoryId, string title, int order, bool isActive, string? icon = null
        )
        {
            Check.NotNullOrWhiteSpace(title, nameof(title));

            var serviceType = await _serviceTypeRepository.GetAsync(id);

            serviceType.CategoryId = categoryId;
            serviceType.Title = title;
            serviceType.Order = order;
            serviceType.IsActive = isActive;
            serviceType.Icon = icon;

            return await _serviceTypeRepository.UpdateAsync(serviceType);
        }

    }
}