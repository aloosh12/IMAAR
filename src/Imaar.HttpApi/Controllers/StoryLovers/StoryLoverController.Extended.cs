using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.StoryLovers;

namespace Imaar.Controllers.StoryLovers
{
    [RemoteService]
    [Area("app")]
    [ControllerName("StoryLover")]
    [Route("api/mobile/story-lovers")]

    public class StoryLoverController : StoryLoverControllerBase, IStoryLoversAppService
    {
        public StoryLoverController(IStoryLoversAppService storyLoversAppService) : base(storyLoversAppService)
        {
        }
    }
}