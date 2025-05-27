using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.FurnishingLevels
{
    public abstract class FurnishingLevelManagerBase : DomainService
    {
        protected IFurnishingLevelRepository _furnishingLevelRepository;

        public FurnishingLevelManagerBase(IFurnishingLevelRepository furnishingLevelRepository)
        {
            _furnishingLevelRepository = furnishingLevelRepository;
        }

        public virtual async Task<FurnishingLevel> CreateAsync(
        string name, bool isActive, int? order = null)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var furnishingLevel = new FurnishingLevel(
             GuidGenerator.Create(),
             name, isActive, order
             );

            return await _furnishingLevelRepository.InsertAsync(furnishingLevel);
        }

        public virtual async Task<FurnishingLevel> UpdateAsync(
            Guid id,
            string name, bool isActive, int? order = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var furnishingLevel = await _furnishingLevelRepository.GetAsync(id);

            furnishingLevel.Name = name;
            furnishingLevel.IsActive = isActive;
            furnishingLevel.Order = order;

            furnishingLevel.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _furnishingLevelRepository.UpdateAsync(furnishingLevel);
        }

    }
}