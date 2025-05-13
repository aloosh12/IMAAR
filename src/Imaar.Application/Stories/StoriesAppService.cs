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

namespace Imaar.Stories
{

    [Authorize(ImaarPermissions.Stories.Default)]
    public abstract class StoriesAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<StoryDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IStoryRepository _storyRepository;
        protected StoryManager _storyManager;

        protected IRepository<Imaar.UserProfiles.UserProfile, Guid> _userProfileRepository;

        public StoriesAppServiceBase(IStoryRepository storyRepository, StoryManager storyManager, IDistributedCache<StoryDownloadTokenCacheItem, string> downloadTokenCache, IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _storyRepository = storyRepository;
            _storyManager = storyManager; _userProfileRepository = userProfileRepository;

        }

        public virtual async Task<PagedResultDto<StoryWithNavigationPropertiesDto>> GetListAsync(GetStoriesInput input)
        {
            var totalCount = await _storyRepository.GetCountAsync(input.FilterText, input.Title, input.FromTimeMin, input.FromTimeMax, input.ExpiryTimeMin, input.ExpiryTimeMax, input.StoryPublisherId);
            var items = await _storyRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Title, input.FromTimeMin, input.FromTimeMax, input.ExpiryTimeMin, input.ExpiryTimeMax, input.StoryPublisherId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<StoryWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<StoryWithNavigationProperties>, List<StoryWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<StoryWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<StoryWithNavigationProperties, StoryWithNavigationPropertiesDto>
                (await _storyRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<StoryDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Story, StoryDto>(await _storyRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input)
        {
            var query = (await _userProfileRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.SecurityNumber != null &&
                         x.SecurityNumber.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.UserProfiles.UserProfile>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.UserProfiles.UserProfile>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(ImaarPermissions.Stories.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _storyRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.Stories.Create)]
        public virtual async Task<StoryDto> CreateAsync(StoryCreateDto input)
        {

            var story = await _storyManager.CreateAsync(
            input.StoryPublisherId, input.FromTime, input.ExpiryTime, input.Title
            );

            return ObjectMapper.Map<Story, StoryDto>(story);
        }

        [Authorize(ImaarPermissions.Stories.Edit)]
        public virtual async Task<StoryDto> UpdateAsync(Guid id, StoryUpdateDto input)
        {

            var story = await _storyManager.UpdateAsync(
            id,
            input.StoryPublisherId, input.FromTime, input.ExpiryTime, input.Title, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Story, StoryDto>(story);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(StoryExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var stories = await _storyRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Title, input.FromTimeMin, input.FromTimeMax, input.ExpiryTimeMin, input.ExpiryTimeMax, input.StoryPublisherId);
            var items = stories.Select(item => new
            {
                Title = item.Story.Title,
                FromTime = item.Story.FromTime,
                ExpiryTime = item.Story.ExpiryTime,

                StoryPublisher = item.StoryPublisher?.SecurityNumber,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Stories.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.Stories.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> storyIds)
        {
            await _storyRepository.DeleteManyAsync(storyIds);
        }

        [Authorize(ImaarPermissions.Stories.Delete)]
        public virtual async Task DeleteAllAsync(GetStoriesInput input)
        {
            await _storyRepository.DeleteAllAsync(input.FilterText, input.Title, input.FromTimeMin, input.FromTimeMax, input.ExpiryTimeMin, input.ExpiryTimeMax, input.StoryPublisherId);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new StoryDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new Imaar.Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}