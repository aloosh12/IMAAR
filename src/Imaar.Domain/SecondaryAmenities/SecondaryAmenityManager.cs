using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.SecondaryAmenities
{
    public abstract class SecondaryAmenityManagerBase : DomainService
    {
        protected ISecondaryAmenityRepository _secondaryAmenityRepository;

        public SecondaryAmenityManagerBase(ISecondaryAmenityRepository secondaryAmenityRepository)
        {
            _secondaryAmenityRepository = secondaryAmenityRepository;
        }

        public virtual async Task<SecondaryAmenity> CreateAsync(
        string name, bool isActive, int? order = null)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var secondaryAmenity = new SecondaryAmenity(
             GuidGenerator.Create(),
             name, isActive, order
             );

            return await _secondaryAmenityRepository.InsertAsync(secondaryAmenity);
        }

        public virtual async Task<SecondaryAmenity> UpdateAsync(
            Guid id,
            string name, bool isActive, int? order = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var secondaryAmenity = await _secondaryAmenityRepository.GetAsync(id);

            secondaryAmenity.Name = name;
            secondaryAmenity.IsActive = isActive;
            secondaryAmenity.Order = order;

            secondaryAmenity.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _secondaryAmenityRepository.UpdateAsync(secondaryAmenity);
        }

    }
}