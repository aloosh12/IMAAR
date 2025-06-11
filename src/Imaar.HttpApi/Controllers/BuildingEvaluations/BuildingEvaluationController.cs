using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.BuildingEvaluations;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.BuildingEvaluations
{
    [RemoteService]
    [Area("app")]
    [ControllerName("BuildingEvaluation")]
    [Route("api/app/notification-types")]

    public abstract class BuildingEvaluationControllerBase : AbpController
    {
        protected IBuildingEvaluationsAppService _buildingEvaluationsAppService;

        public BuildingEvaluationControllerBase(IBuildingEvaluationsAppService buildingEvaluationsAppService)
        {
            _buildingEvaluationsAppService = buildingEvaluationsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<BuildingEvaluationDto>> GetListAsync(GetBuildingEvaluationsInput input)
        {
            return _buildingEvaluationsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<BuildingEvaluationDto> GetAsync(Guid id)
        {
            return _BuildingEvaluationsAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<BuildingEvaluationDto> CreateAsync(BuildingEvaluationCreateDto input)
        {
            return _BuildingEvaluationsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<BuildingEvaluationDto> UpdateAsync(Guid id, BuildingEvaluationUpdateDto input)
        {
            return _BuildingEvaluationsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _BuildingEvaluationsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(BuildingEvaluationExcelDownloadDto input)
        {
            return _BuildingEvaluationsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _BuildingEvaluationsAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> BuildingEvaluationIds)
        {
            return _BuildingEvaluationsAppService.DeleteByIdsAsync(BuildingEvaluationIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetBuildingEvaluationsInput input)
        {
            return _BuildingEvaluationsAppService.DeleteAllAsync(input);
        }
    }
}