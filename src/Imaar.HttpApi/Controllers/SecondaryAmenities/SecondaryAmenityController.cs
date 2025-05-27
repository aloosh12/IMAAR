using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.SecondaryAmenities;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.SecondaryAmenities
{
    [RemoteService]
    [Area("app")]
    [ControllerName("SecondaryAmenity")]
    [Route("api/app/secondary-amenities")]

    public abstract class SecondaryAmenityControllerBase : AbpController
    {
        protected ISecondaryAmenitiesAppService _secondaryAmenitiesAppService;

        public SecondaryAmenityControllerBase(ISecondaryAmenitiesAppService secondaryAmenitiesAppService)
        {
            _secondaryAmenitiesAppService = secondaryAmenitiesAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<SecondaryAmenityDto>> GetListAsync(GetSecondaryAmenitiesInput input)
        {
            return _secondaryAmenitiesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<SecondaryAmenityDto> GetAsync(Guid id)
        {
            return _secondaryAmenitiesAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<SecondaryAmenityDto> CreateAsync(SecondaryAmenityCreateDto input)
        {
            return _secondaryAmenitiesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<SecondaryAmenityDto> UpdateAsync(Guid id, SecondaryAmenityUpdateDto input)
        {
            return _secondaryAmenitiesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _secondaryAmenitiesAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(SecondaryAmenityExcelDownloadDto input)
        {
            return _secondaryAmenitiesAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _secondaryAmenitiesAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> secondaryamenityIds)
        {
            return _secondaryAmenitiesAppService.DeleteByIdsAsync(secondaryamenityIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetSecondaryAmenitiesInput input)
        {
            return _secondaryAmenitiesAppService.DeleteAllAsync(input);
        }
    }
}