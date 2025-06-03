using Asp.Versioning;
using Imaar.StoryTickets;
using Imaar.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;

namespace Imaar.Controllers.StoryTickets
{
    [RemoteService]
    [Area("app")]
    [ControllerName("StoryTicket")]
    [Route("api/mobile/story-tickets")]

    public class StoryTicketController : AbpController
    {
        protected IStoryTicketsAppService _storyTicketsAppService;

        public StoryTicketController(IStoryTicketsAppService storyTicketsAppService)
        {
            _storyTicketsAppService = storyTicketsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<StoryTicketWithNavigationPropertiesDto>> GetListAsync(GetStoryTicketsInput input)
        {
            return _storyTicketsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<StoryTicketWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _storyTicketsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<StoryTicketDto> GetAsync(Guid id)
        {
            return _storyTicketsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("story-ticket-type-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetStoryTicketTypeLookupAsync(LookupRequestDto input)
        {
            return _storyTicketsAppService.GetStoryTicketTypeLookupAsync(input);
        }

        [HttpGet]
        [Route("user-profile-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input)
        {
            return _storyTicketsAppService.GetUserProfileLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<StoryTicketDto> CreateAsync(StoryTicketCreateDto input)
        {
            return _storyTicketsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<StoryTicketDto> UpdateAsync(Guid id, StoryTicketUpdateDto input)
        {
            return _storyTicketsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _storyTicketsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(StoryTicketExcelDownloadDto input)
        {
            return _storyTicketsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _storyTicketsAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> storyticketIds)
        {
            return _storyTicketsAppService.DeleteByIdsAsync(storyticketIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetStoryTicketsInput input)
        {
            return _storyTicketsAppService.DeleteAllAsync(input);
        }
    }
}