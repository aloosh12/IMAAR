using Imaar.Medias;
using Imaar.MobileResponses;
using Imaar.Permissions;
using Imaar.Shared;
using Imaar.Shared;
using Imaar.Stories;
using Imaar.StoryLovers;
using Imaar.UserFollows;
using Imaar.UserProfiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Imaar.Stories
{
    public class StoriesAppService : StoriesAppServiceBase, IStoriesAppService
    {
        // protected IMediasAppService _mediasAppService;

        protected IStoryLoversAppService _storyLoversAppService;
        protected IStoryLoverRepository _storyLoverRepository;
        protected ICurrentUser _currentUser;
        public StoriesAppService(IStoryRepository storyRepository, StoryManager storyManager, 
            IDistributedCache<StoryDownloadTokenCacheItem, string> downloadTokenCache, 
            IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository,
            IMediasAppService mediasAppService, IStoryLoversAppService storyLoversAppService, IStoryLoverRepository storyLoverRepository ,ICurrentUser currentUser)
            : base(storyRepository, storyManager, downloadTokenCache, userProfileRepository, mediasAppService)
        {
            _storyLoversAppService = storyLoversAppService;
            _storyLoverRepository = storyLoverRepository;
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
        
        public virtual async Task<bool> CheckLoveStatusAsync(Guid id)
        {
            var currentUserId = _currentUser.Id;
            if (currentUserId == null)
            {
                return false;
            }
            
            // Check if the current user has loved this story
            var input = new GetStoryLoversInput
            {
                UserProfileId = currentUserId,
                StoryId = id,
                MaxResultCount = 1
            };
            var totalCount = await _storyLoverRepository.GetCountAsync(input.FilterText, input.UserProfileId, input.StoryId);
            return totalCount > 0;
        }
        
        public virtual async Task<long> GetLoveCountAsync(Guid id)
        {
            // Get count of loves for the story
            var input = new GetStoryLoversInput
            {
                StoryId = id,
                MaxResultCount = 1  // We only need count, not the actual items
            };

            return await _storyLoverRepository.GetCountAsync(input.FilterText, input.UserProfileId, input.StoryId);
            //var result = await _storyLoversAppService.GetListAsync(input);
            //return (int)result.TotalCount;
        }

        public virtual async Task<bool> UnfollowStoryAsync(Guid id)
        {
            var currentUserId = _currentUser.Id;
            if (currentUserId == null)
            {
                throw new UserFriendlyException("Current user not exist");
            }
            var storyLove = await _storyLoverRepository.FirstOrDefaultAsync(x => x.UserProfileId == currentUserId && x.StoryId == id); ;
            if (storyLove == null)
            {
                throw new UserFriendlyException("Story Love not exist");
            }
            await _storyLoversAppService.DeleteAsync(storyLove.Id);
            return true;
            //var result = await _storyLoversAppService.GetListAsync(input);
            //return (int)result.TotalCount;
        }

        //public virtual async Task<PagedResultDto<StoryMobileDto>> GetStoriesLovedByUserAsync(Guid userId, int skipCount = 0, int maxResultCount = 10)
        //public virtual async Task<PagedResultDto<StoryMobileDto>> GetCurrentUserLovedStoriesAsync(int skipCount = 0, int maxResultCount = 10)

        [AllowAnonymous]
        public virtual async Task<MobileResponseDto> CreateWithFilesAsync(StoryCreateWithFilesDto input)
        {
            var result = await _storyManager.CreateWithFilesAsync(input.Title, input.PublisherId, input.Files);
            return ObjectMapper.Map<MobileResponse, MobileResponseDto>(result);
        }
    }
}