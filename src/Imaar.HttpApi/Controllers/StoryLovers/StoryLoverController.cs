using Imaar.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.StoryLovers;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.StoryLovers
{
    [RemoteService]
    [Area("app")]
    [ControllerName("StoryLover")]
    [Route("api/app/story-lovers")]

    public abstract class StoryLoverControllerBase : AbpController
    {
        protected IStoryLoversAppService _storyLoversAppService;

        public StoryLoverControllerBase(IStoryLoversAppService storyLoversAppService)
        {
            _storyLoversAppService = storyLoversAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<StoryLoverWithNavigationPropertiesDto>> GetListAsync(GetStoryLoversInput input)
        {
            return _storyLoversAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<StoryLoverWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _storyLoversAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<StoryLoverDto> GetAsync(Guid id)
        {
            return _storyLoversAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("user-profile-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input)
        {
            return _storyLoversAppService.GetUserProfileLookupAsync(input);
        }

        [HttpGet]
        [Route("story-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetStoryLookupAsync(LookupRequestDto input)
        {
            return _storyLoversAppService.GetStoryLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<StoryLoverDto> CreateAsync(StoryLoverCreateDto input)
        {
            return _storyLoversAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<StoryLoverDto> UpdateAsync(Guid id, StoryLoverUpdateDto input)
        {
            return _storyLoversAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _storyLoversAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(StoryLoverExcelDownloadDto input)
        {
            return _storyLoversAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _storyLoversAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> storyloverIds)
        {
            return _storyLoversAppService.DeleteByIdsAsync(storyloverIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetStoryLoversInput input)
        {
            return _storyLoversAppService.DeleteAllAsync(input);
        }
    }
}