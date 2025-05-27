using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.Regions
{
    public abstract class RegionManagerBase : DomainService
    {
        protected IRegionRepository _regionRepository;

        public RegionManagerBase(IRegionRepository regionRepository)
        {
            _regionRepository = regionRepository;
        }

        public virtual async Task<Region> CreateAsync(
        Guid cityId, string name, bool isActive, int? order = null)
        {
            Check.NotNull(cityId, nameof(cityId));
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var region = new Region(
             GuidGenerator.Create(),
             cityId, name, isActive, order
             );

            return await _regionRepository.InsertAsync(region);
        }

        public virtual async Task<Region> UpdateAsync(
            Guid id,
            Guid cityId, string name, bool isActive, int? order = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(cityId, nameof(cityId));
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var region = await _regionRepository.GetAsync(id);

            region.CityId = cityId;
            region.Name = name;
            region.IsActive = isActive;
            region.Order = order;

            region.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _regionRepository.UpdateAsync(region);
        }

    }
}