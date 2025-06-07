using Imaar.Shared;
using Imaar.UserProfiles;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Imaar.Permissions;
using Imaar.Stories;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;
using Imaar.MobileResponses;
using Imaar.Medias;

namespace Imaar.Stories
{
    public class StoriesAppService : StoriesAppServiceBase, IStoriesAppService
    {
       // protected IMediasAppService _mediasAppService;
        
        public StoriesAppService(IStoryRepository storyRepository, StoryManager storyManager, 
            IDistributedCache<StoryDownloadTokenCacheItem, string> downloadTokenCache, 
            IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository,
            IMediasAppService mediasAppService)
            : base(storyRepository, storyManager, downloadTokenCache, userProfileRepository, mediasAppService)
        {
        }

        //Write your custom code...
        public virtual async Task<PagedResultDto<StoryMobileDto>> GetMobileListAsync(GetStoriesInput input)
        {
            var totalCount = await _storyRepository.GetCountAsync(input.FilterText, input.Title, input.FromTimeMin, input.FromTimeMax, input.ExpiryTimeMin, input.ExpiryTimeMax, input.StoryPublisherId);
            var items = await _storyRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Title, input.FromTimeMin, input.FromTimeMax, input.ExpiryTimeMin, input.ExpiryTimeMax, input.StoryPublisherId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<StoryMobileDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<StoryWithNavigationProperties>, List<StoryMobileDto>>(items)
            };
        }

        [AllowAnonymous]
        public virtual async Task<MobileResponseDto> CreateWithFilesAsync(StoryCreateWithFilesDto input)
        {
            var result = await _storyManager.CreateWithFilesAsync(input.Title, input.PublisherId, input.Files);
            return ObjectMapper.Map<MobileResponse, MobileResponseDto>(result);
        }
    }
}