using Imaar.Shared;
using Imaar.SecondaryAmenities;
using Imaar.MainAmenities;
using Imaar.ServiceTypes;
using Imaar.BuildingFacades;
using Imaar.FurnishingLevels;
using Imaar.Regions;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Imaar.Permissions;
using Imaar.Buildings;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;
using Imaar.MobileResponses;
using Imaar.Medias;

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
            IRepository<Imaar.MainAmenities.MainAmenity, Guid> mainAmenityRepository, 
            IRepository<Imaar.SecondaryAmenities.SecondaryAmenity, Guid> secondaryAmenityRepository,
            IMediasAppService mediasAppService)
            : base(buildingRepository, buildingManager, downloadTokenCache, regionRepository, furnishingLevelRepository, buildingFacadeRepository, serviceTypeRepository, mainAmenityRepository, secondaryAmenityRepository)
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
                input.MainAmenityIds,
                input.SecondaryAmenityIds,
                input.Files
            );
            
            return ObjectMapper.Map<MobileResponse, MobileResponseDto>(result);
        }
    }
}