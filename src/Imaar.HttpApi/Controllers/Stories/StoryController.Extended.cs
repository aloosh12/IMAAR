using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.Stories;
using Imaar.MobileResponses;
using Microsoft.AspNetCore.Authorization;

namespace Imaar.Controllers.Stories
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Story")]
    [Route("api/mobile/stories")]

    public class StoryController : StoryControllerBase, IStoriesAppService
    {
        public StoryController(IStoriesAppService storiesAppService) : base(storiesAppService)
        {
        }
        
        [HttpGet("mobile-list")]
        public Task<PagedResultDto<StoryMobileDto>> GetMobileListAsync(GetStoriesInput input)
        {
            return _storiesAppService.GetMobileListAsync(input);
        }
        
        [HttpPost("create-with-files")]
        public virtual Task<MobileResponseDto> CreateWithFilesAsync([FromForm] StoryCreateWithFilesDto input)
        {
            return _storiesAppService.CreateWithFilesAsync(input);
        }
    }
}