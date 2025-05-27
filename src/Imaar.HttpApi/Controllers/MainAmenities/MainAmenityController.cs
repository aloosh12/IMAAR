using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.MainAmenities;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.MainAmenities
{
    [RemoteService]
    [Area("app")]
    [ControllerName("MainAmenity")]
    [Route("api/app/main-amenities")]

    public abstract class MainAmenityControllerBase : AbpController
    {
        protected IMainAmenitiesAppService _mainAmenitiesAppService;

        public MainAmenityControllerBase(IMainAmenitiesAppService mainAmenitiesAppService)
        {
            _mainAmenitiesAppService = mainAmenitiesAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<MainAmenityDto>> GetListAsync(GetMainAmenitiesInput input)
        {
            return _mainAmenitiesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<MainAmenityDto> GetAsync(Guid id)
        {
            return _mainAmenitiesAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<MainAmenityDto> CreateAsync(MainAmenityCreateDto input)
        {
            return _mainAmenitiesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<MainAmenityDto> UpdateAsync(Guid id, MainAmenityUpdateDto input)
        {
            return _mainAmenitiesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _mainAmenitiesAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(MainAmenityExcelDownloadDto input)
        {
            return _mainAmenitiesAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _mainAmenitiesAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> mainamenityIds)
        {
            return _mainAmenitiesAppService.DeleteByIdsAsync(mainamenityIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetMainAmenitiesInput input)
        {
            return _mainAmenitiesAppService.DeleteAllAsync(input);
        }
    }
}