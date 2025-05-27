using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.FurnishingLevels;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.FurnishingLevels
{
    [RemoteService]
    [Area("app")]
    [ControllerName("FurnishingLevel")]
    [Route("api/app/furnishing-levels")]

    public abstract class FurnishingLevelControllerBase : AbpController
    {
        protected IFurnishingLevelsAppService _furnishingLevelsAppService;

        public FurnishingLevelControllerBase(IFurnishingLevelsAppService furnishingLevelsAppService)
        {
            _furnishingLevelsAppService = furnishingLevelsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<FurnishingLevelDto>> GetListAsync(GetFurnishingLevelsInput input)
        {
            return _furnishingLevelsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<FurnishingLevelDto> GetAsync(Guid id)
        {
            return _furnishingLevelsAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<FurnishingLevelDto> CreateAsync(FurnishingLevelCreateDto input)
        {
            return _furnishingLevelsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<FurnishingLevelDto> UpdateAsync(Guid id, FurnishingLevelUpdateDto input)
        {
            return _furnishingLevelsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _furnishingLevelsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(FurnishingLevelExcelDownloadDto input)
        {
            return _furnishingLevelsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _furnishingLevelsAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> furnishinglevelIds)
        {
            return _furnishingLevelsAppService.DeleteByIdsAsync(furnishinglevelIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetFurnishingLevelsInput input)
        {
            return _furnishingLevelsAppService.DeleteAllAsync(input);
        }
    }
}