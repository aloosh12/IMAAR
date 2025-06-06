using AutoMapper.Internal.Mappers;
using Imaar.BuildingFacades;
using Imaar.Buildings;
using Imaar.Buildings;
using Imaar.FurnishingLevels;
using Imaar.MainAmenities;
using Imaar.Permissions;
using Imaar.Regions;
using Imaar.SecondaryAmenities;
using Imaar.ServiceTypes;
using Imaar.Shared;
using Imaar.Shared;
using Imaar.Shared;
using Imaar.UserProfiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;

namespace Imaar.Buildings
{

    [Authorize(ImaarPermissions.Buildings.Default)]
    public abstract class BuildingsAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<BuildingDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IBuildingRepository _buildingRepository;
        protected BuildingManager _buildingManager;

        protected IRepository<Imaar.Regions.Region, Guid> _regionRepository;
        protected IRepository<Imaar.FurnishingLevels.FurnishingLevel, Guid> _furnishingLevelRepository;
        protected IRepository<Imaar.BuildingFacades.BuildingFacade, Guid> _buildingFacadeRepository;
        protected IRepository<Imaar.ServiceTypes.ServiceType, Guid> _serviceTypeRepository;
        protected IRepository<Imaar.UserProfiles.UserProfile, Guid> _userProfileRepository;
        protected IRepository<Imaar.MainAmenities.MainAmenity, Guid> _mainAmenityRepository;
        protected IRepository<Imaar.SecondaryAmenities.SecondaryAmenity, Guid> _secondaryAmenityRepository;

        public BuildingsAppServiceBase(IBuildingRepository buildingRepository, BuildingManager buildingManager, IDistributedCache<BuildingDownloadTokenCacheItem, string> downloadTokenCache, IRepository<Imaar.Regions.Region, Guid> regionRepository, IRepository<Imaar.FurnishingLevels.FurnishingLevel, Guid> furnishingLevelRepository, IRepository<Imaar.BuildingFacades.BuildingFacade, Guid> buildingFacadeRepository, IRepository<Imaar.ServiceTypes.ServiceType, Guid> serviceTypeRepository, IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository, IRepository<Imaar.MainAmenities.MainAmenity, Guid> mainAmenityRepository, IRepository<Imaar.SecondaryAmenities.SecondaryAmenity, Guid> secondaryAmenityRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _buildingRepository = buildingRepository;
            _buildingManager = buildingManager; _regionRepository = regionRepository;
            _furnishingLevelRepository = furnishingLevelRepository;
            _buildingFacadeRepository = buildingFacadeRepository;
            _serviceTypeRepository = serviceTypeRepository;
            _userProfileRepository = userProfileRepository;
            _mainAmenityRepository = mainAmenityRepository;
            _secondaryAmenityRepository = secondaryAmenityRepository;

        }

        public virtual async Task<PagedResultDto<BuildingWithNavigationPropertiesDto>> GetListAsync(GetBuildingsInput input)
        {
            var totalCount = await _buildingRepository.GetCountAsync(input.FilterText, input.MainTitle, input.Description, input.Price, input.BuildingArea, input.NumberOfRooms, input.NumberOfBaths, input.FloorNo, input.ViewCounterMin, input.ViewCounterMax, input.OrderCounterMin, input.OrderCounterMax, input.RegionId, input.FurnishingLevelId, input.BuildingFacadeId, input.ServiceTypeId, input.UserProfileId, input.MainAmenityId, input.SecondaryAmenityId);
            var items = await _buildingRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.MainTitle, input.Description, input.Price, input.BuildingArea, input.NumberOfRooms, input.NumberOfBaths, input.FloorNo, input.ViewCounterMin, input.ViewCounterMax, input.OrderCounterMin, input.OrderCounterMax, input.RegionId, input.FurnishingLevelId, input.BuildingFacadeId, input.ServiceTypeId, input.UserProfileId, input.MainAmenityId, input.SecondaryAmenityId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<BuildingWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<BuildingWithNavigationProperties>, List<BuildingWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<BuildingWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<BuildingWithNavigationProperties, BuildingWithNavigationPropertiesDto>
                (await _buildingRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<BuildingDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Building, BuildingDto>(await _buildingRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetRegionLookupAsync(LookupRequestDto input)
        {
            var query = (await _regionRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null &&
                         x.Name.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.Regions.Region>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.Regions.Region>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetFurnishingLevelLookupAsync(LookupRequestDto input)
        {
            var query = (await _furnishingLevelRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null &&
                         x.Name.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.FurnishingLevels.FurnishingLevel>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.FurnishingLevels.FurnishingLevel>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetBuildingFacadeLookupAsync(LookupRequestDto input)
        {
            var query = (await _buildingFacadeRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null &&
                         x.Name.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.BuildingFacades.BuildingFacade>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.BuildingFacades.BuildingFacade>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetServiceTypeLookupAsync(LookupRequestDto input)
        {
            var query = (await _serviceTypeRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Title != null &&
                         x.Title.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.ServiceTypes.ServiceType>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.ServiceTypes.ServiceType>, List<LookupDto<Guid>>>(lookupData)
            };
        }
        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input)
        {
            var query = (await _userProfileRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.SecurityNumber != null &&
                         x.SecurityNumber.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.UserProfiles.UserProfile>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.UserProfiles.UserProfile>, List<LookupDto<Guid>>>(lookupData)
            };
        }
        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetMainAmenityLookupAsync(LookupRequestDto input)
        {
            var query = (await _mainAmenityRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null &&
                         x.Name.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.MainAmenities.MainAmenity>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.MainAmenities.MainAmenity>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetSecondaryAmenityLookupAsync(LookupRequestDto input)
        {
            var query = (await _secondaryAmenityRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null &&
                         x.Name.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.SecondaryAmenities.SecondaryAmenity>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.SecondaryAmenities.SecondaryAmenity>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(ImaarPermissions.Buildings.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _buildingRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.Buildings.Create)]
        public virtual async Task<BuildingDto> CreateAsync(BuildingCreateDto input)
        {
            if (input.RegionId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Region"]]);
            }
            if (input.FurnishingLevelId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["FurnishingLevel"]]);
            }
            if (input.BuildingFacadeId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["BuildingFacade"]]);
            }
            if (input.ServiceTypeId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["ServiceType"]]);
            }

            var building = await _buildingManager.CreateAsync(
            input.MainAmenityIds, input.SecondaryAmenityIds, input.RegionId, input.FurnishingLevelId, input.BuildingFacadeId, input.ServiceTypeId, input.UserProfileId, input.MainTitle, input.Description, input.Price, input.BuildingArea, input.NumberOfRooms, input.NumberOfBaths, input.FloorNo, input.ViewCounter, input.OrderCounter
            );

            return ObjectMapper.Map<Building, BuildingDto>(building);
        }

        [Authorize(ImaarPermissions.Buildings.Edit)]
        public virtual async Task<BuildingDto> UpdateAsync(Guid id, BuildingUpdateDto input)
        {
            if (input.RegionId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Region"]]);
            }
            if (input.FurnishingLevelId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["FurnishingLevel"]]);
            }
            if (input.BuildingFacadeId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["BuildingFacade"]]);
            }
            if (input.ServiceTypeId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["ServiceType"]]);
            }

            var building = await _buildingManager.UpdateAsync(
            id,
            input.MainAmenityIds, input.SecondaryAmenityIds, input.RegionId, input.FurnishingLevelId, input.BuildingFacadeId, input.ServiceTypeId, input.UserProfileId, input.MainTitle, input.Description, input.Price, input.BuildingArea, input.NumberOfRooms, input.NumberOfBaths, input.FloorNo, input.ViewCounter, input.OrderCounter, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Building, BuildingDto>(building);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(BuildingExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var buildings = await _buildingRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.MainTitle, input.Description, input.Price, input.BuildingArea, input.NumberOfRooms, input.NumberOfBaths, input.FloorNo, input.ViewCounterMin, input.ViewCounterMax, input.OrderCounterMin, input.OrderCounterMax, input.RegionId, input.FurnishingLevelId, input.BuildingFacadeId, input.ServiceTypeId, input.MainAmenityId, input.SecondaryAmenityId);
            var items = buildings.Select(item => new
            {
                MainTitle = item.Building.MainTitle,
                Description = item.Building.Description,
                Price = item.Building.Price,
                BuildingArea = item.Building.BuildingArea,
                NumberOfRooms = item.Building.NumberOfRooms,
                NumberOfBaths = item.Building.NumberOfBaths,
                FloorNo = item.Building.FloorNo,
                ViewCounter = item.Building.ViewCounter,
                OrderCounter = item.Building.OrderCounter,

                Region = item.Region?.Name,
                FurnishingLevel = item.FurnishingLevel?.Name,
                BuildingFacade = item.BuildingFacade?.Name,
                ServiceType = item.ServiceType?.Title,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Buildings.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.Buildings.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> buildingIds)
        {
            await _buildingRepository.DeleteManyAsync(buildingIds);
        }

        [Authorize(ImaarPermissions.Buildings.Delete)]
        public virtual async Task DeleteAllAsync(GetBuildingsInput input)
        {
            await _buildingRepository.DeleteAllAsync(input.FilterText, input.MainTitle, input.Description, input.Price, input.BuildingArea, input.NumberOfRooms, input.NumberOfBaths, input.FloorNo, input.ViewCounterMin, input.ViewCounterMax, input.OrderCounterMin, input.OrderCounterMax, input.RegionId, input.FurnishingLevelId, input.BuildingFacadeId, input.ServiceTypeId, input.MainAmenityId, input.SecondaryAmenityId);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new BuildingDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new Imaar.Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}