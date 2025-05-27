using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.Cities
{
    public abstract class CityManagerBase : DomainService
    {
        protected ICityRepository _cityRepository;

        public CityManagerBase(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public virtual async Task<City> CreateAsync(
        string name, bool isActive, int? order = null)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var city = new City(
             GuidGenerator.Create(),
             name, isActive, order
             );

            return await _cityRepository.InsertAsync(city);
        }

        public virtual async Task<City> UpdateAsync(
            Guid id,
            string name, bool isActive, int? order = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var city = await _cityRepository.GetAsync(id);

            city.Name = name;
            city.IsActive = isActive;
            city.Order = order;

            city.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _cityRepository.UpdateAsync(city);
        }

    }
}