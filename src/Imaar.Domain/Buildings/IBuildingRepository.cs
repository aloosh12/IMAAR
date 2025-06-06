using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.Buildings
{
    public partial interface IBuildingRepository : IRepository<Building, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? mainTitle = null,
            string? description = null,
            string? price = null,
            string? buildingArea = null,
            string? numberOfRooms = null,
            string? numberOfBaths = null,
            string? floorNo = null,
            int? viewCounterMin = null,
            int? viewCounterMax = null,
            int? orderCounterMin = null,
            int? orderCounterMax = null,
            Guid? regionId = null,
            Guid? furnishingLevelId = null,
            Guid? buildingFacadeId = null,
            Guid? serviceTypeId = null,
            Guid? userProfileId = null,
            Guid? mainAmenityId = null,
            Guid? secondaryAmenityId = null,
            CancellationToken cancellationToken = default);
        Task<BuildingWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<BuildingWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? mainTitle = null,
            string? description = null,
            string? price = null,
            string? buildingArea = null,
            string? numberOfRooms = null,
            string? numberOfBaths = null,
            string? floorNo = null,
            int? viewCounterMin = null,
            int? viewCounterMax = null,
            int? orderCounterMin = null,
            int? orderCounterMax = null,
            Guid? regionId = null,
            Guid? furnishingLevelId = null,
            Guid? buildingFacadeId = null,
            Guid? serviceTypeId = null,
              Guid? userProfileId = null,
            Guid? mainAmenityId = null,
            Guid? secondaryAmenityId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<Building>> GetListAsync(
                    string? filterText = null,
                    string? mainTitle = null,
                    string? description = null,
                    string? price = null,
                    string? buildingArea = null,
                    string? numberOfRooms = null,
                    string? numberOfBaths = null,
                    string? floorNo = null,
                    int? viewCounterMin = null,
                    int? viewCounterMax = null,
                    int? orderCounterMin = null,
                    int? orderCounterMax = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? mainTitle = null,
            string? description = null,
            string? price = null,
            string? buildingArea = null,
            string? numberOfRooms = null,
            string? numberOfBaths = null,
            string? floorNo = null,
            int? viewCounterMin = null,
            int? viewCounterMax = null,
            int? orderCounterMin = null,
            int? orderCounterMax = null,
            Guid? regionId = null,
            Guid? furnishingLevelId = null,
            Guid? buildingFacadeId = null,
            Guid? serviceTypeId = null,
              Guid? userProfileId = null,
            Guid? mainAmenityId = null,
            Guid? secondaryAmenityId = null,
            CancellationToken cancellationToken = default);
    }
}