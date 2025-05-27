using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.MainAmenities
{
    public abstract class MainAmenityManagerBase : DomainService
    {
        protected IMainAmenityRepository _mainAmenityRepository;

        public MainAmenityManagerBase(IMainAmenityRepository mainAmenityRepository)
        {
            _mainAmenityRepository = mainAmenityRepository;
        }

        public virtual async Task<MainAmenity> CreateAsync(
        string name, bool isActive, int? order = null)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var mainAmenity = new MainAmenity(
             GuidGenerator.Create(),
             name, isActive, order
             );

            return await _mainAmenityRepository.InsertAsync(mainAmenity);
        }

        public virtual async Task<MainAmenity> UpdateAsync(
            Guid id,
            string name, bool isActive, int? order = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var mainAmenity = await _mainAmenityRepository.GetAsync(id);

            mainAmenity.Name = name;
            mainAmenity.IsActive = isActive;
            mainAmenity.Order = order;

            mainAmenity.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _mainAmenityRepository.UpdateAsync(mainAmenity);
        }

    }
}