using Imaar.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.Medias;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.Medias
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Media")]
    [Route("api/app/medias")]

    public abstract class MediaControllerBase : AbpController
    {
        protected IMediasAppService _mediasAppService;

        public MediaControllerBase(IMediasAppService mediasAppService)
        {
            _mediasAppService = mediasAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<MediaWithNavigationPropertiesDto>> GetListAsync(GetMediasInput input)
        {
            return _mediasAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<MediaWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _mediasAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<MediaDto> GetAsync(Guid id)
        {
            return _mediasAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("imaar-service-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetImaarServiceLookupAsync(LookupRequestDto input)
        {
            return _mediasAppService.GetImaarServiceLookupAsync(input);
        }

        [HttpGet]
        [Route("vacancy-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetVacancyLookupAsync(LookupRequestDto input)
        {
            return _mediasAppService.GetVacancyLookupAsync(input);
        }

        [HttpGet]
        [Route("story-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetStoryLookupAsync(LookupRequestDto input)
        {
            return _mediasAppService.GetStoryLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<MediaDto> CreateAsync(MediaCreateDto input)
        {
            return _mediasAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<MediaDto> UpdateAsync(Guid id, MediaUpdateDto input)
        {
            return _mediasAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _mediasAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(MediaExcelDownloadDto input)
        {
            return _mediasAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _mediasAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> mediaIds)
        {
            return _mediasAppService.DeleteByIdsAsync(mediaIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetMediasInput input)
        {
            return _mediasAppService.DeleteAllAsync(input);
        }
    }
}