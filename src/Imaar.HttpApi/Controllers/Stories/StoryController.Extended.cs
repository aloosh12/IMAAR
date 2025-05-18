using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.Stories;

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
            throw new NotImplementedException();
        }
    }
}