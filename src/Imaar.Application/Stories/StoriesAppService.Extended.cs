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
using Imaar.StoryLovers;
using Volo.Abp.Users;

namespace Imaar.Stories
{
    public class StoriesAppService : StoriesAppServiceBase, IStoriesAppService
    {
        // protected IMediasAppService _mediasAppService;

        protected IStoryLoversAppService _storyLoversAppService;
        protected ICurrentUser _currentUser;
        public StoriesAppService(IStoryRepository storyRepository, StoryManager storyManager, 
            IDistributedCache<StoryDownloadTokenCacheItem, string> downloadTokenCache, 
            IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository,
            IMediasAppService mediasAppService, IStoryLoversAppService storyLoversAppService, ICurrentUser currentUser)
            : base(storyRepository, storyManager, downloadTokenCache, userProfileRepository, mediasAppService)
        {
            _storyLoversAppService = storyLoversAppService;
            _currentUser = currentUser;
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

        //public virtual async Task<PagedResultDto<StoryMobileDto>> GetStoriesLovedByUserAsync(Guid userId, int skipCount = 0, int maxResultCount = 10)
        //{
        //    // Get the story lovers records for the specified user
        //    var getStoryLoversInput = new GetStoryLoversInput
        //    {
        //        UserProfileId = userId,
        //        MaxResultCount = maxResultCount,
        //        SkipCount = skipCount
        //    };
            
        //    var storyLoversResult = await _storyLoversAppService.GetListAsync(getStoryLoversInput);
            
        //    if (storyLoversResult.TotalCount == 0)
        //    {
        //        return new PagedResultDto<StoryMobileDto>
        //        {
        //            TotalCount = 0,
        //            Items = new List<StoryMobileDto>()
        //        };
        //    }
            
        //    // Extract story IDs from the story lovers records
        //    var storyIds = storyLoversResult.Items.Select(sl => sl.StoryId).ToList();
            
        //    // Get the actual story objects with their navigation properties (including media)
        //    var storiesWithNavProperties = await _storyRepository.GetListWithNavigationPropertiesByStoryIdsAsync(storyIds);
            
        //    // Map to mobile DTOs
        //    var storyDtos = ObjectMapper.Map<List<StoryWithNavigationProperties>, List<StoryMobileDto>>(storiesWithNavProperties);
            
        //    return new PagedResultDto<StoryMobileDto>
        //    {
        //        TotalCount = storyLoversResult.TotalCount,
        //        Items = storyDtos
        //    };
        //}
        
        // Convenience method that uses the current user's ID
        //public virtual async Task<PagedResultDto<StoryMobileDto>> GetCurrentUserLovedStoriesAsync(int skipCount = 0, int maxResultCount = 10)
        //{
        //    if (_currentUser?.Id == null)
        //    {
        //        throw new Volo.Abp.UserFriendlyException("User is not authenticated");
        //    }
            
        //    return await GetStoriesLovedByUserAsync(_currentUser.Id.Value, skipCount, maxResultCount);
        //}

        //public virtual async Task<PagedResultDto<StoryMobileDto>> GetStoryLovedByUserAsync()
        //{
        //    GetStoryLoversInput input = new GetStoryLoversInput();
        //    input.SkipCount = 0;
        //    input.MaxResultCount = 1000;
        //    input.UserProfileId = _currentUser.Id;
        //    PagedResultDto _storyLoversAppService.GetListAsync(input);
        //    var totalCount = await _storyRepository.GetCountAsync(input.FilterText, input.Title, input.FromTimeMin, input.FromTimeMax, input.ExpiryTimeMin, input.ExpiryTimeMax, input.StoryPublisherId);
        //    var items = await _storyRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Title, input.FromTimeMin, input.FromTimeMax, input.ExpiryTimeMin, input.ExpiryTimeMax, input.StoryPublisherId, input.Sorting, input.MaxResultCount, input.SkipCount);

        //    return new PagedResultDto<StoryMobileDto>
        //    {
        //        TotalCount = totalCount,
        //        Items = ObjectMapper.Map<List<StoryWithNavigationProperties>, List<StoryMobileDto>>(items)
        //    };
        //}

        [AllowAnonymous]
        public virtual async Task<MobileResponseDto> CreateWithFilesAsync(StoryCreateWithFilesDto input)
        {
            var result = await _storyManager.CreateWithFilesAsync(input.Title, input.PublisherId, input.Files);
            return ObjectMapper.Map<MobileResponse, MobileResponseDto>(result);
        }
    }
}