using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.BuildingFacades;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.BuildingFacades
{
    [RemoteService]
    [Area("app")]
    [ControllerName("BuildingFacade")]
    [Route("api/app/building-facades")]

    public abstract class BuildingFacadeControllerBase : AbpController
    {
        protected IBuildingFacadesAppService _buildingFacadesAppService;

        public BuildingFacadeControllerBase(IBuildingFacadesAppService buildingFacadesAppService)
        {
            _buildingFacadesAppService = buildingFacadesAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<BuildingFacadeDto>> GetListAsync(GetBuildingFacadesInput input)
        {
            return _buildingFacadesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<BuildingFacadeDto> GetAsync(Guid id)
        {
            return _buildingFacadesAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<BuildingFacadeDto> CreateAsync(BuildingFacadeCreateDto input)
        {
            return _buildingFacadesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<BuildingFacadeDto> UpdateAsync(Guid id, BuildingFacadeUpdateDto input)
        {
            return _buildingFacadesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _buildingFacadesAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(BuildingFacadeExcelDownloadDto input)
        {
            return _buildingFacadesAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _buildingFacadesAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> buildingfacadeIds)
        {
            return _buildingFacadesAppService.DeleteByIdsAsync(buildingfacadeIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetBuildingFacadesInput input)
        {
            return _buildingFacadesAppService.DeleteAllAsync(input);
        }
    }
}