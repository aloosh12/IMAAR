using Imaar.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.Buildings;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.Buildings
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Building")]
    [Route("api/app/buildings")]

    public abstract class BuildingControllerBase : AbpController
    {
        protected IBuildingsAppService _buildingsAppService;

        public BuildingControllerBase(IBuildingsAppService buildingsAppService)
        {
            _buildingsAppService = buildingsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<BuildingWithNavigationPropertiesDto>> GetListAsync(GetBuildingsInput input)
        {
            return _buildingsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<BuildingWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _buildingsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<BuildingDto> GetAsync(Guid id)
        {
            return _buildingsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("region-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetRegionLookupAsync(LookupRequestDto input)
        {
            return _buildingsAppService.GetRegionLookupAsync(input);
        }

        [HttpGet]
        [Route("furnishing-level-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetFurnishingLevelLookupAsync(LookupRequestDto input)
        {
            return _buildingsAppService.GetFurnishingLevelLookupAsync(input);
        }

        [HttpGet]
        [Route("building-facade-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetBuildingFacadeLookupAsync(LookupRequestDto input)
        {
            return _buildingsAppService.GetBuildingFacadeLookupAsync(input);
        }

        [HttpGet]
        [Route("service-type-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetServiceTypeLookupAsync(LookupRequestDto input)
        {
            return _buildingsAppService.GetServiceTypeLookupAsync(input);
        }

        [HttpGet]
        [Route("main-amenity-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetMainAmenityLookupAsync(LookupRequestDto input)
        {
            return _buildingsAppService.GetMainAmenityLookupAsync(input);
        }

        [HttpGet]
        [Route("secondary-amenity-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetSecondaryAmenityLookupAsync(LookupRequestDto input)
        {
            return _buildingsAppService.GetSecondaryAmenityLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<BuildingDto> CreateAsync(BuildingCreateDto input)
        {
            return _buildingsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<BuildingDto> UpdateAsync(Guid id, BuildingUpdateDto input)
        {
            return _buildingsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _buildingsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(BuildingExcelDownloadDto input)
        {
            return _buildingsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _buildingsAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> buildingIds)
        {
            return _buildingsAppService.DeleteByIdsAsync(buildingIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetBuildingsInput input)
        {
            return _buildingsAppService.DeleteAllAsync(input);
        }
    }
}