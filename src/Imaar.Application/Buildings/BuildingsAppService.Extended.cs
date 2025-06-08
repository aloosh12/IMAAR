using Imaar.BuildingFacades;
using Imaar.Buildings;
using Imaar.FurnishingLevels;
using Imaar.ImaarServices;
using Imaar.MainAmenities;
using Imaar.Medias;
using Imaar.MobileResponses;
using Imaar.Permissions;
using Imaar.Regions;
using Imaar.SecondaryAmenities;
using Imaar.ServiceTypes;
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
    public class BuildingsAppService : BuildingsAppServiceBase, IBuildingsAppService
    {
        protected IMediasAppService _mediasAppService;
        
        public BuildingsAppService(
            IBuildingRepository buildingRepository, 
            BuildingManager buildingManager, 
            IDistributedCache<BuildingDownloadTokenCacheItem, string> downloadTokenCache, 
            IRepository<Imaar.Regions.Region, Guid> regionRepository, 
            IRepository<Imaar.FurnishingLevels.FurnishingLevel, Guid> furnishingLevelRepository, 
            IRepository<Imaar.BuildingFacades.BuildingFacade, Guid> buildingFacadeRepository, 
            IRepository<Imaar.ServiceTypes.ServiceType, Guid> serviceTypeRepository, 
            IUserProfileRepository userProfileRepository, 
            IRepository<Imaar.MainAmenities.MainAmenity, Guid> mainAmenityRepository, 
            IRepository<Imaar.SecondaryAmenities.SecondaryAmenity, Guid> secondaryAmenityRepository,
            IMediasAppService mediasAppService)
            : base(buildingRepository, buildingManager, downloadTokenCache, regionRepository, furnishingLevelRepository, buildingFacadeRepository, serviceTypeRepository, userProfileRepository, mainAmenityRepository, secondaryAmenityRepository)
        {
            _mediasAppService = mediasAppService;
        }

        [AllowAnonymous]
        public virtual async Task<MobileResponseDto> CreateWithFilesAsync(BuildingCreateWithFilesDto input)
        {
            var result = await _buildingManager.CreateWithFilesAsync(
                input.MainTitle,
                input.Description,
                input.Price,
                input.BuildingArea,
                input.NumberOfRooms,
                input.NumberOfBaths,
                input.FloorNo,
                input.RegionId,
                input.FurnishingLevelId,
                input.BuildingFacadeId,
                input.ServiceTypeId,
                input.UserProfileId,
                input.MainAmenityIds,
                input.SecondaryAmenityIds,
                input.Files
            );
            
            return ObjectMapper.Map<MobileResponse, MobileResponseDto>(result);
        }
        
        [AllowAnonymous]
        public virtual async Task<MobileResponseDto> GetBuildingWithDetailsAsync(Guid id)
        {
            var mobileResponse = new MobileResponse();
            
            try
            {
                // Get the Building with navigation properties
                var buildingWithDetails = await GetWithNavigationPropertiesAsync(id);
                
                if (buildingWithDetails == null || buildingWithDetails.Building == null)
                {
                    mobileResponse.Code = 404;
                    mobileResponse.Message = "Building not found";
                    mobileResponse.Data = null;
                    
                    return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
                }
                
                // Map to DTOs
                //var buildingDto = ObjectMapper.Map<Building, BuildingDto>(buildingWithDetails.Building);
                //var serviceTypeDto = ObjectMapper.Map<ServiceType, ServiceTypeDto>(buildingWithDetails.ServiceType);
                //var regionDto = ObjectMapper.Map<Region, RegionDto>(buildingWithDetails.Region);
                //var furnishingLevelDto = ObjectMapper.Map<FurnishingLevel, FurnishingLevelDto>(buildingWithDetails.FurnishingLevel);
                //var buildingFacadeDto = ObjectMapper.Map<BuildingFacade, BuildingFacadeDto>(buildingWithDetails.BuildingFacade);
                
                // Get media for this building
                var getMediasInput = new GetMediasInput
                {
                    SkipCount = 0,
                    MaxResultCount = 1000,
                    SourceEntityId = id.ToString(),
                    SourceEntityType = MediaEntityType.Building,
                    IsActive = true,
                    Sorting = "Order asc"
                };
                var mediaListDto = await _mediasAppService.GetListAsync(getMediasInput);

                // Create result DTO
                var usertemp = ObjectMapper.Map<UserProfileWithDetails, UserProfileWithDetailsDto>(await _userProfileRepository.GetWithDetailsAsync(buildingWithDetails.UserProfile.Id));

                var result = new BuildingWithDetailsMobileDto
                {
                    // Copy properties from buildingDto
                    
                    
                    // Add navigation properties
                    Building = buildingWithDetails.Building,
                    ServiceType = buildingWithDetails.ServiceType,
                    UserProfileWithDetailsDto = usertemp,
                    Region = buildingWithDetails.Region,
                    FurnishingLevel = buildingWithDetails.FurnishingLevel,
                    BuildingFacade = buildingWithDetails.BuildingFacade,
                    MainAmenities = buildingWithDetails.MainAmenities,
                    SecondaryAmenities = buildingWithDetails.SecondaryAmenities,
                    Media = mediaListDto != null ? mediaListDto.Items.ToList() : new List<MediaDto>()
                };
                
                // Increment the view counter asynchronously (fire and forget)
                _ = IncrementViewCounterAsync(id);
                
                mobileResponse.Code = 200;
                mobileResponse.Message = "Building retrieved successfully";
                mobileResponse.Data = result;
                
                return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
            }
            catch (Exception ex)
            {
                mobileResponse.Code = 500;
                mobileResponse.Message = ex.Message;
                mobileResponse.Data = null;
                
                return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
            }
        }
        
        [AllowAnonymous]
        public virtual async Task<MobileResponseDto> IncrementViewCounterAsync(Guid id)
        {
            var mobileResponse = new MobileResponse();
            
            try
            {
                // Get the Building
                var building = await _buildingRepository.GetAsync(id);
                
                if (building == null)
                {
                    mobileResponse.Code = 404;
                    mobileResponse.Message = "Building not found";
                    mobileResponse.Data = null;
                    
                    return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
                }
                
                // Increment the view counter
                building.ViewCounter += 1;
                
                // Update the entity
                await _buildingRepository.UpdateAsync(building);
                
                mobileResponse.Code = 200;
                mobileResponse.Message = "View counter incremented successfully";
                mobileResponse.Data = new { 
                    BuildingId = building.Id, 
                    ViewCounter = building.ViewCounter 
                };
                
                return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
            }
            catch (Exception ex)
            {
                mobileResponse.Code = 500;
                mobileResponse.Message = ex.Message;
                mobileResponse.Data = null;
                
                return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
            }
        }
        
        [AllowAnonymous]
        public virtual async Task<MobileResponseDto> IncrementOrderCounterAsync(Guid id)
        {
            var mobileResponse = new MobileResponse();
            
            try
            {
                // Get the Building
                var building = await _buildingRepository.GetAsync(id);
                
                if (building == null)
                {
                    mobileResponse.Code = 404;
                    mobileResponse.Message = "Building not found";
                    mobileResponse.Data = null;
                    
                    return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
                }
                
                // Increment the order counter
                building.OrderCounter += 1;
                
                // Update the entity
                await _buildingRepository.UpdateAsync(building);
                
                mobileResponse.Code = 200;
                mobileResponse.Message = "Order counter incremented successfully";
                mobileResponse.Data = new { 
                    BuildingId = building.Id, 
                    OrderCounter = building.OrderCounter 
                };
                
                return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
            }
            catch (Exception ex)
            {
                mobileResponse.Code = 500;
                mobileResponse.Message = ex.Message;
                mobileResponse.Data = null;
                
                return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
            }
        }
    }
}