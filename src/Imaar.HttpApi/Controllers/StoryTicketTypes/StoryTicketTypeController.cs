using Asp.Versioning;
using Imaar.ServiceTicketTypes;
using Imaar.Shared;
using Imaar.StoryTicketTypes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;

namespace Imaar.Controllers.StoryTicketTypes
{
    [RemoteService]
    [Area("app")]
    [ControllerName("StoryTicketType")]
    [Route("api/mobile/story-ticket-types")]

    public class StoryTicketTypeController 
    {
        protected IStoryTicketTypesAppService _storyTicketTypesAppService;

        public StoryTicketTypeController(IStoryTicketTypesAppService storyTicketTypesAppService)
        {
            _storyTicketTypesAppService = storyTicketTypesAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<StoryTicketTypeDto>> GetListAsync(GetStoryTicketTypesInput input)
        {
            return _storyTicketTypesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<StoryTicketTypeDto> GetAsync(Guid id)
        {
            return _storyTicketTypesAppService.GetAsync(id);
        }
    }
}