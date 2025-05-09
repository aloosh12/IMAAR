using Imaar.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.ImaarServices;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.ImaarServices
{
    [RemoteService]
    [Area("app")]
    [ControllerName("ImaarService")]
    [Route("api/app/imaar-services")]

    public abstract class ImaarServiceControllerBase : AbpController
    {
        protected IImaarServicesAppService _imaarServicesAppService;

        public ImaarServiceControllerBase(IImaarServicesAppService imaarServicesAppService)
        {
            _imaarServicesAppService = imaarServicesAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<ImaarServiceWithNavigationPropertiesDto>> GetListAsync(GetImaarServicesInput input)
        {
            return _imaarServicesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<ImaarServiceWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _imaarServicesAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<ImaarServiceDto> GetAsync(Guid id)
        {
            return _imaarServicesAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("service-type-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetServiceTypeLookupAsync(LookupRequestDto input)
        {
            return _imaarServicesAppService.GetServiceTypeLookupAsync(input);
        }

        [HttpGet]
        [Route("user-profile-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input)
        {
            return _imaarServicesAppService.GetUserProfileLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<ImaarServiceDto> CreateAsync(ImaarServiceCreateDto input)
        {
            return _imaarServicesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<ImaarServiceDto> UpdateAsync(Guid id, ImaarServiceUpdateDto input)
        {
            return _imaarServicesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _imaarServicesAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(ImaarServiceExcelDownloadDto input)
        {
            return _imaarServicesAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _imaarServicesAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> imaarserviceIds)
        {
            return _imaarServicesAppService.DeleteByIdsAsync(imaarserviceIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetImaarServicesInput input)
        {
            return _imaarServicesAppService.DeleteAllAsync(input);
        }
    }
}