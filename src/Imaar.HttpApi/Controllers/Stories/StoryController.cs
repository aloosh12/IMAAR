using Imaar.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.Stories;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.Stories
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Story")]
    [Route("api/app/stories")]

    public abstract class StoryControllerBase : AbpController
    {
        protected IStoriesAppService _storiesAppService;

        public StoryControllerBase(IStoriesAppService storiesAppService)
        {
            _storiesAppService = storiesAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<StoryWithNavigationPropertiesDto>> GetListAsync(GetStoriesInput input)
        {
            return _storiesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<StoryWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _storiesAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<StoryDto> GetAsync(Guid id)
        {
            return _storiesAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("user-profile-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input)
        {
            return _storiesAppService.GetUserProfileLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<StoryDto> CreateAsync(StoryCreateDto input)
        {
            return _storiesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<StoryDto> UpdateAsync(Guid id, StoryUpdateDto input)
        {
            return _storiesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _storiesAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(StoryExcelDownloadDto input)
        {
            return _storiesAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _storiesAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> storyIds)
        {
            return _storiesAppService.DeleteByIdsAsync(storyIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetStoriesInput input)
        {
            return _storiesAppService.DeleteAllAsync(input);
        }
    }
}