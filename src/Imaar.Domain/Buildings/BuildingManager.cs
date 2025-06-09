using Imaar.Buildings;
using Imaar.MainAmenities;
using Imaar.SecondaryAmenities;
using Imaar.MainAmenities;
using Imaar.SecondaryAmenities;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Imaar.Buildings
{
    public abstract class BuildingManagerBase : DomainService
    {
        protected IBuildingRepository _buildingRepository;
        protected IRepository<MainAmenity, Guid> _mainAmenityRepository;
        protected IRepository<SecondaryAmenity, Guid> _secondaryAmenityRepository;

        public BuildingManagerBase(IBuildingRepository buildingRepository,
        IRepository<MainAmenity, Guid> mainAmenityRepository,
        IRepository<SecondaryAmenity, Guid> secondaryAmenityRepository)
        {
            _buildingRepository = buildingRepository;
            _mainAmenityRepository = mainAmenityRepository;
            _secondaryAmenityRepository = secondaryAmenityRepository;
        }

        public virtual async Task<Building> CreateAsync(
        List<Guid> mainAmenityIds,
        List<Guid> secondaryAmenityIds,
        Guid regionId, Guid furnishingLevelId, Guid buildingFacadeId, Guid serviceTypeId, Guid userProfileId, string mainTitle, string description, string price, string buildingArea, string numberOfRooms, string numberOfBaths, string floorNo, int viewCounter, int orderCounter, string? latitude = null, string? longitude = null)
        {
            Check.NotNull(regionId, nameof(regionId));
            Check.NotNull(furnishingLevelId, nameof(furnishingLevelId));
            Check.NotNull(buildingFacadeId, nameof(buildingFacadeId));
            Check.NotNull(serviceTypeId, nameof(serviceTypeId));
            Check.NotNull(userProfileId, nameof(userProfileId));
            Check.NotNullOrWhiteSpace(mainTitle, nameof(mainTitle));
            Check.NotNullOrWhiteSpace(description, nameof(description));
            Check.NotNullOrWhiteSpace(price, nameof(price));
            Check.NotNullOrWhiteSpace(buildingArea, nameof(buildingArea));
            Check.NotNullOrWhiteSpace(numberOfRooms, nameof(numberOfRooms));
            Check.NotNullOrWhiteSpace(numberOfBaths, nameof(numberOfBaths));
            Check.NotNullOrWhiteSpace(floorNo, nameof(floorNo));

            var building = new Building(
             GuidGenerator.Create(),
             regionId, furnishingLevelId, buildingFacadeId, serviceTypeId, userProfileId, mainTitle, description, price, buildingArea, numberOfRooms, numberOfBaths, floorNo, viewCounter, orderCounter, latitude, longitude
             );

            await SetMainAmenitiesAsync(building, mainAmenityIds);
            await SetSecondaryAmenitiesAsync(building, secondaryAmenityIds);

            return await _buildingRepository.InsertAsync(building);
        }

        public virtual async Task<Building> UpdateAsync(
            Guid id,
            List<Guid> mainAmenityIds,
        List<Guid> secondaryAmenityIds,
        Guid regionId, Guid furnishingLevelId, Guid buildingFacadeId, Guid serviceTypeId, Guid userProfileId, string mainTitle, string description, string price, string buildingArea, string numberOfRooms, string numberOfBaths, string floorNo, int viewCounter, int orderCounter, string? latitude = null, string? longitude = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(regionId, nameof(regionId));
            Check.NotNull(furnishingLevelId, nameof(furnishingLevelId));
            Check.NotNull(buildingFacadeId, nameof(buildingFacadeId));
            Check.NotNull(serviceTypeId, nameof(serviceTypeId));
            Check.NotNull(userProfileId, nameof(userProfileId));
            Check.NotNullOrWhiteSpace(mainTitle, nameof(mainTitle));
            Check.NotNullOrWhiteSpace(description, nameof(description));
            Check.NotNullOrWhiteSpace(price, nameof(price));
            Check.NotNullOrWhiteSpace(buildingArea, nameof(buildingArea));
            Check.NotNullOrWhiteSpace(numberOfRooms, nameof(numberOfRooms));
            Check.NotNullOrWhiteSpace(numberOfBaths, nameof(numberOfBaths));
            Check.NotNullOrWhiteSpace(floorNo, nameof(floorNo));

            var queryable = await _buildingRepository.WithDetailsAsync(x => x.MainAmenities, x => x.SecondaryAmenities);
            var query = queryable.Where(x => x.Id == id);

            var building = await AsyncExecuter.FirstOrDefaultAsync(query);

            building.RegionId = regionId;
            building.FurnishingLevelId = furnishingLevelId;
            building.BuildingFacadeId = buildingFacadeId;
            building.ServiceTypeId = serviceTypeId;
            building.UserProfileId = userProfileId;
            building.MainTitle = mainTitle;
            building.Description = description;
            building.Price = price;
            building.BuildingArea = buildingArea;
            building.NumberOfRooms = numberOfRooms;
            building.NumberOfBaths = numberOfBaths;
            building.FloorNo = floorNo;
            building.ViewCounter = viewCounter;
            building.OrderCounter = orderCounter;
            building.Latitude = latitude;
            building.Longitude = longitude;

            await SetMainAmenitiesAsync(building, mainAmenityIds);
            await SetSecondaryAmenitiesAsync(building, secondaryAmenityIds);

            building.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _buildingRepository.UpdateAsync(building);
        }

        private async Task SetMainAmenitiesAsync(Building building, List<Guid> mainAmenityIds)
        {
            if (mainAmenityIds == null || !mainAmenityIds.Any())
            {
                building.RemoveAllMainAmenities();
                return;
            }

            var query = (await _mainAmenityRepository.GetQueryableAsync())
                .Where(x => mainAmenityIds.Contains(x.Id))
                .Select(x => x.Id);

            var mainAmenityIdsInDb = await AsyncExecuter.ToListAsync(query);
            if (!mainAmenityIdsInDb.Any())
            {
                return;
            }

            building.RemoveAllMainAmenitiesExceptGivenIds(mainAmenityIdsInDb);

            foreach (var mainAmenityId in mainAmenityIdsInDb)
            {
                building.AddMainAmenity(mainAmenityId);
            }
        }

        private async Task SetSecondaryAmenitiesAsync(Building building, List<Guid> secondaryAmenityIds)
        {
            if (secondaryAmenityIds == null || !secondaryAmenityIds.Any())
            {
                building.RemoveAllSecondaryAmenities();
                return;
            }

            var query = (await _secondaryAmenityRepository.GetQueryableAsync())
                .Where(x => secondaryAmenityIds.Contains(x.Id))
                .Select(x => x.Id);

            var secondaryAmenityIdsInDb = await AsyncExecuter.ToListAsync(query);
            if (!secondaryAmenityIdsInDb.Any())
            {
                return;
            }

            building.RemoveAllSecondaryAmenitiesExceptGivenIds(secondaryAmenityIdsInDb);

            foreach (var secondaryAmenityId in secondaryAmenityIdsInDb)
            {
                building.AddSecondaryAmenity(secondaryAmenityId);
            }
        }

    }
}