using Imaar.BuildingFacades;
using Imaar.Buildings;
using Imaar.FurnishingLevels;
using Imaar.MainAmenities;
using Imaar.Regions;
using Imaar.SecondaryAmenities;
using Imaar.ServiceTypes;
using Imaar.UserProfiles;
using Imaar.BuildingFacades;
using Imaar.EntityFrameworkCore;
using Imaar.FurnishingLevels;
using Imaar.MainAmenities;
using Imaar.MainAmenities;
using Imaar.Regions;
using Imaar.SecondaryAmenities;
using Imaar.SecondaryAmenities;
using Imaar.ServiceTypes;
using Imaar.UserProfiles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Imaar.Buildings
{
    public abstract class EfCoreBuildingRepositoryBase : EfCoreRepository<ImaarDbContext, Building, Guid>
    {
        public EfCoreBuildingRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? mainTitle = null,
            string? description = null,
            string? price = null,
            string? buildingArea = null,
            string? numberOfRooms = null,
            string? numberOfBaths = null,
            string? floorNo = null,
            string? latitude = null,
            string? longitude = null,
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
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, mainTitle, description, price, buildingArea, numberOfRooms, numberOfBaths, floorNo, latitude, longitude, viewCounterMin, viewCounterMax, orderCounterMin, orderCounterMax, regionId, furnishingLevelId, buildingFacadeId, serviceTypeId, userProfileId, mainAmenityId, secondaryAmenityId);

            var ids = query.Select(x => x.Building.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<BuildingWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id).Include(x => x.MainAmenities).Include(x => x.SecondaryAmenities)
                .Select(building => new BuildingWithNavigationProperties
                {
                    Building = building,
                    Region = dbContext.Set<Region>().FirstOrDefault(c => c.Id == building.RegionId),
                    FurnishingLevel = dbContext.Set<FurnishingLevel>().FirstOrDefault(c => c.Id == building.FurnishingLevelId),
                    BuildingFacade = dbContext.Set<BuildingFacade>().FirstOrDefault(c => c.Id == building.BuildingFacadeId),
                    ServiceType = dbContext.Set<ServiceType>().FirstOrDefault(c => c.Id == building.ServiceTypeId),
                    UserProfile = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == building.UserProfileId),
                    MainAmenities = (from buildingMainAmenities in building.MainAmenities
                                     join _mainAmenity in dbContext.Set<MainAmenity>() on buildingMainAmenities.MainAmenityId equals _mainAmenity.Id
                                     select _mainAmenity).ToList(),
                    SecondaryAmenities = (from buildingSecondaryAmenities in building.SecondaryAmenities
                                          join _secondaryAmenity in dbContext.Set<SecondaryAmenity>() on buildingSecondaryAmenities.SecondaryAmenityId equals _secondaryAmenity.Id
                                          select _secondaryAmenity).ToList()
                }).FirstOrDefault();
        }

        public virtual async Task<List<BuildingWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? mainTitle = null,
            string? description = null,
            string? price = null,
            string? buildingArea = null,
            string? numberOfRooms = null,
            string? numberOfBaths = null,
            string? floorNo = null,
            string? latitude = null,
            string? longitude = null,
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
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, mainTitle, description, price, buildingArea, numberOfRooms, numberOfBaths, floorNo, latitude, longitude, viewCounterMin, viewCounterMax, orderCounterMin, orderCounterMax, regionId, furnishingLevelId, buildingFacadeId, serviceTypeId, userProfileId, mainAmenityId, secondaryAmenityId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? BuildingConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<BuildingWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from building in (await GetDbSetAsync())
                   join region in (await GetDbContextAsync()).Set<Region>() on building.RegionId equals region.Id into regions
                   from region in regions.DefaultIfEmpty()
                   join furnishingLevel in (await GetDbContextAsync()).Set<FurnishingLevel>() on building.FurnishingLevelId equals furnishingLevel.Id into furnishingLevels
                   from furnishingLevel in furnishingLevels.DefaultIfEmpty()
                   join buildingFacade in (await GetDbContextAsync()).Set<BuildingFacade>() on building.BuildingFacadeId equals buildingFacade.Id into buildingFacades
                   from buildingFacade in buildingFacades.DefaultIfEmpty()
                   join serviceType in (await GetDbContextAsync()).Set<ServiceType>() on building.ServiceTypeId equals serviceType.Id into serviceTypes
                   from serviceType in serviceTypes.DefaultIfEmpty()
                   join userProfile in (await GetDbContextAsync()).Set<UserProfile>() on building.UserProfileId equals userProfile.Id into userProfiles
                   from userProfile in userProfiles.DefaultIfEmpty()
                   select new BuildingWithNavigationProperties
                   {
                       Building = building,
                       Region = region,
                       FurnishingLevel = furnishingLevel,
                       BuildingFacade = buildingFacade,
                       ServiceType = serviceType,
                       UserProfile = userProfile,
                       MainAmenities = new List<MainAmenity>(),
                       SecondaryAmenities = new List<SecondaryAmenity>()
                   };
        }

        protected virtual IQueryable<BuildingWithNavigationProperties> ApplyFilter(
            IQueryable<BuildingWithNavigationProperties> query,
            string? filterText,
            string? mainTitle = null,
            string? description = null,
            string? price = null,
            string? buildingArea = null,
            string? numberOfRooms = null,
            string? numberOfBaths = null,
            string? floorNo = null,
            string? latitude = null,
            string? longitude = null,
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
            Guid? secondaryAmenityId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Building.MainTitle!.Contains(filterText!) || e.Building.Description!.Contains(filterText!) || e.Building.Price!.Contains(filterText!) || e.Building.BuildingArea!.Contains(filterText!) || e.Building.NumberOfRooms!.Contains(filterText!) || e.Building.NumberOfBaths!.Contains(filterText!) || e.Building.FloorNo!.Contains(filterText!) || e.Building.Latitude!.Contains(filterText!) || e.Building.Longitude!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(mainTitle), e => e.Building.MainTitle.Contains(mainTitle))
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Building.Description.Contains(description))
                    .WhereIf(!string.IsNullOrWhiteSpace(price), e => e.Building.Price.Contains(price))
                    .WhereIf(!string.IsNullOrWhiteSpace(buildingArea), e => e.Building.BuildingArea.Contains(buildingArea))
                    .WhereIf(!string.IsNullOrWhiteSpace(numberOfRooms), e => e.Building.NumberOfRooms.Contains(numberOfRooms))
                    .WhereIf(!string.IsNullOrWhiteSpace(numberOfBaths), e => e.Building.NumberOfBaths.Contains(numberOfBaths))
                    .WhereIf(!string.IsNullOrWhiteSpace(floorNo), e => e.Building.FloorNo.Contains(floorNo))
                    .WhereIf(!string.IsNullOrWhiteSpace(latitude), e => e.Building.Latitude.Contains(latitude))
                    .WhereIf(!string.IsNullOrWhiteSpace(longitude), e => e.Building.Longitude.Contains(longitude))
                    .WhereIf(viewCounterMin.HasValue, e => e.Building.ViewCounter >= viewCounterMin!.Value)
                    .WhereIf(viewCounterMax.HasValue, e => e.Building.ViewCounter <= viewCounterMax!.Value)
                    .WhereIf(orderCounterMin.HasValue, e => e.Building.OrderCounter >= orderCounterMin!.Value)
                    .WhereIf(orderCounterMax.HasValue, e => e.Building.OrderCounter <= orderCounterMax!.Value)
                    .WhereIf(regionId != null && regionId != Guid.Empty, e => e.Region != null && e.Region.Id == regionId)
                    .WhereIf(furnishingLevelId != null && furnishingLevelId != Guid.Empty, e => e.FurnishingLevel != null && e.FurnishingLevel.Id == furnishingLevelId)
                    .WhereIf(buildingFacadeId != null && buildingFacadeId != Guid.Empty, e => e.BuildingFacade != null && e.BuildingFacade.Id == buildingFacadeId)
                    .WhereIf(serviceTypeId != null && serviceTypeId != Guid.Empty, e => e.ServiceType != null && e.ServiceType.Id == serviceTypeId)
                    .WhereIf(userProfileId != null && userProfileId != Guid.Empty, e => e.UserProfile != null && e.UserProfile.Id == userProfileId)
                    .WhereIf(mainAmenityId != null && mainAmenityId != Guid.Empty, e => e.Building.MainAmenities.Any(x => x.MainAmenityId == mainAmenityId))
                    .WhereIf(secondaryAmenityId != null && secondaryAmenityId != Guid.Empty, e => e.Building.SecondaryAmenities.Any(x => x.SecondaryAmenityId == secondaryAmenityId));
        }

        public virtual async Task<List<Building>> GetListAsync(
            string? filterText = null,
            string? mainTitle = null,
            string? description = null,
            string? price = null,
            string? buildingArea = null,
            string? numberOfRooms = null,
            string? numberOfBaths = null,
            string? floorNo = null,
            string? latitude = null,
            string? longitude = null,
            int? viewCounterMin = null,
            int? viewCounterMax = null,
            int? orderCounterMin = null,
            int? orderCounterMax = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, mainTitle, description, price, buildingArea, numberOfRooms, numberOfBaths, floorNo, latitude, longitude, viewCounterMin, viewCounterMax, orderCounterMin, orderCounterMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? BuildingConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? mainTitle = null,
            string? description = null,
            string? price = null,
            string? buildingArea = null,
            string? numberOfRooms = null,
            string? numberOfBaths = null,
            string? floorNo = null,
            string? latitude = null,
            string? longitude = null,
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
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, mainTitle, description, price, buildingArea, numberOfRooms, numberOfBaths, floorNo, latitude, longitude, viewCounterMin, viewCounterMax, orderCounterMin, orderCounterMax, regionId, furnishingLevelId, buildingFacadeId, serviceTypeId, userProfileId, mainAmenityId, secondaryAmenityId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Building> ApplyFilter(
            IQueryable<Building> query,
            string? filterText = null,
            string? mainTitle = null,
            string? description = null,
            string? price = null,
            string? buildingArea = null,
            string? numberOfRooms = null,
            string? numberOfBaths = null,
            string? floorNo = null,
            string? latitude = null,
            string? longitude = null,
            int? viewCounterMin = null,
            int? viewCounterMax = null,
            int? orderCounterMin = null,
            int? orderCounterMax = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.MainTitle!.Contains(filterText!) || e.Description!.Contains(filterText!) || e.Price!.Contains(filterText!) || e.BuildingArea!.Contains(filterText!) || e.NumberOfRooms!.Contains(filterText!) || e.NumberOfBaths!.Contains(filterText!) || e.FloorNo!.Contains(filterText!) || e.Latitude!.Contains(filterText!) || e.Longitude!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(mainTitle), e => e.MainTitle.Contains(mainTitle))
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description))
                    .WhereIf(!string.IsNullOrWhiteSpace(price), e => e.Price.Contains(price))
                    .WhereIf(!string.IsNullOrWhiteSpace(buildingArea), e => e.BuildingArea.Contains(buildingArea))
                    .WhereIf(!string.IsNullOrWhiteSpace(numberOfRooms), e => e.NumberOfRooms.Contains(numberOfRooms))
                    .WhereIf(!string.IsNullOrWhiteSpace(numberOfBaths), e => e.NumberOfBaths.Contains(numberOfBaths))
                    .WhereIf(!string.IsNullOrWhiteSpace(floorNo), e => e.FloorNo.Contains(floorNo))
                    .WhereIf(!string.IsNullOrWhiteSpace(latitude), e => e.Latitude.Contains(latitude))
                    .WhereIf(!string.IsNullOrWhiteSpace(longitude), e => e.Longitude.Contains(longitude))
                    .WhereIf(viewCounterMin.HasValue, e => e.ViewCounter >= viewCounterMin!.Value)
                    .WhereIf(viewCounterMax.HasValue, e => e.ViewCounter <= viewCounterMax!.Value)
                    .WhereIf(orderCounterMin.HasValue, e => e.OrderCounter >= orderCounterMin!.Value)
                    .WhereIf(orderCounterMax.HasValue, e => e.OrderCounter <= orderCounterMax!.Value);
        }
    }
}