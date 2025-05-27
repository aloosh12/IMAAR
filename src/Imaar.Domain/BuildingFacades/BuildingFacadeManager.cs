using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.BuildingFacades
{
    public abstract class BuildingFacadeManagerBase : DomainService
    {
        protected IBuildingFacadeRepository _buildingFacadeRepository;

        public BuildingFacadeManagerBase(IBuildingFacadeRepository buildingFacadeRepository)
        {
            _buildingFacadeRepository = buildingFacadeRepository;
        }

        public virtual async Task<BuildingFacade> CreateAsync(
        string name, bool isActive, int? order = null)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var buildingFacade = new BuildingFacade(
             GuidGenerator.Create(),
             name, isActive, order
             );

            return await _buildingFacadeRepository.InsertAsync(buildingFacade);
        }

        public virtual async Task<BuildingFacade> UpdateAsync(
            Guid id,
            string name, bool isActive, int? order = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var buildingFacade = await _buildingFacadeRepository.GetAsync(id);

            buildingFacade.Name = name;
            buildingFacade.IsActive = isActive;
            buildingFacade.Order = order;

            buildingFacade.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _buildingFacadeRepository.UpdateAsync(buildingFacade);
        }

    }
}