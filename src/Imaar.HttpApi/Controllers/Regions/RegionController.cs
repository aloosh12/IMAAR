using Imaar.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.Regions;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.Regions
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Region")]
    [Route("api/app/regions")]

    public abstract class RegionControllerBase : AbpController
    {
        protected IRegionsAppService _regionsAppService;

        public RegionControllerBase(IRegionsAppService regionsAppService)
        {
            _regionsAppService = regionsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<RegionWithNavigationPropertiesDto>> GetListAsync(GetRegionsInput input)
        {
            return _regionsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<RegionWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _regionsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<RegionDto> GetAsync(Guid id)
        {
            return _regionsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("city-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetCityLookupAsync(LookupRequestDto input)
        {
            return _regionsAppService.GetCityLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<RegionDto> CreateAsync(RegionCreateDto input)
        {
            return _regionsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<RegionDto> UpdateAsync(Guid id, RegionUpdateDto input)
        {
            return _regionsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _regionsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(RegionExcelDownloadDto input)
        {
            return _regionsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _regionsAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> regionIds)
        {
            return _regionsAppService.DeleteByIdsAsync(regionIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetRegionsInput input)
        {
            return _regionsAppService.DeleteAllAsync(input);
        }
    }
}