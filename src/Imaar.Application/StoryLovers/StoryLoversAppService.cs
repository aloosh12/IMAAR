using Imaar.Shared;
using Imaar.Stories;
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
using Imaar.StoryLovers;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.StoryLovers
{

    [Authorize(ImaarPermissions.StoryLovers.Default)]
    public abstract class StoryLoversAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<StoryLoverDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IStoryLoverRepository _storyLoverRepository;
        protected StoryLoverManager _storyLoverManager;

        protected IRepository<Imaar.UserProfiles.UserProfile, Guid> _userProfileRepository;
        protected IRepository<Imaar.Stories.Story, Guid> _storyRepository;

        public StoryLoversAppServiceBase(IStoryLoverRepository storyLoverRepository, StoryLoverManager storyLoverManager, IDistributedCache<StoryLoverDownloadTokenCacheItem, string> downloadTokenCache, IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository, IRepository<Imaar.Stories.Story, Guid> storyRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _storyLoverRepository = storyLoverRepository;
            _storyLoverManager = storyLoverManager; _userProfileRepository = userProfileRepository;
            _storyRepository = storyRepository;

        }

        public virtual async Task<PagedResultDto<StoryLoverWithNavigationPropertiesDto>> GetListAsync(GetStoryLoversInput input)
        {
            var totalCount = await _storyLoverRepository.GetCountAsync(input.FilterText, input.UserProfileId, input.StoryId);
            var items = await _storyLoverRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.UserProfileId, input.StoryId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<StoryLoverWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<StoryLoverWithNavigationProperties>, List<StoryLoverWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<StoryLoverWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<StoryLoverWithNavigationProperties, StoryLoverWithNavigationPropertiesDto>
                (await _storyLoverRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<StoryLoverDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<StoryLover, StoryLoverDto>(await _storyLoverRepository.GetAsync(id));
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

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetStoryLookupAsync(LookupRequestDto input)
        {
            var query = (await _storyRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Title != null &&
                         x.Title.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.Stories.Story>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.Stories.Story>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(ImaarPermissions.StoryLovers.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _storyLoverRepository.DeleteAsync(id);
        }
        
        public virtual async Task<bool> StoryLoverExistsAsync(Guid userProfileId, Guid storyId)
        {
            // Check if a story lover record already exists for this user and story
            var query = await GetListAsync(new GetStoryLoversInput
            {
                UserProfileId = userProfileId,
                StoryId = storyId,
                MaxResultCount = 1
            });
            
            return query.TotalCount > 0;
        }

        [Authorize(ImaarPermissions.StoryLovers.Create)]
        public virtual async Task<StoryLoverDto> CreateAsync(StoryLoverCreateDto input)
        {
            if (input.UserProfileId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.StoryId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Story"]]);
            }
            
            // Check if this user already loves this story
            if (await StoryLoverExistsAsync(input.UserProfileId, input.StoryId))
            {
                // User already loves this story, return existing record
                throw new UserFriendlyException("You already Love this story");
            }

            var storyLover = await _storyLoverManager.CreateAsync(
            input.UserProfileId, input.StoryId
            );

            return ObjectMapper.Map<StoryLover, StoryLoverDto>(storyLover);
        }

        [Authorize(ImaarPermissions.StoryLovers.Edit)]
        public virtual async Task<StoryLoverDto> UpdateAsync(Guid id, StoryLoverUpdateDto input)
        {
            if (input.UserProfileId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.StoryId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Story"]]);
            }

            var storyLover = await _storyLoverManager.UpdateAsync(
            id,
            input.UserProfileId, input.StoryId, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<StoryLover, StoryLoverDto>(storyLover);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(StoryLoverExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var storyLovers = await _storyLoverRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.UserProfileId, input.StoryId);
            var items = storyLovers.Select(item => new
            {

                UserProfile = item.UserProfile?.SecurityNumber,
                Story = item.Story?.Title,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "StoryLovers.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.StoryLovers.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> storyloverIds)
        {
            await _storyLoverRepository.DeleteManyAsync(storyloverIds);
        }

        [Authorize(ImaarPermissions.StoryLovers.Delete)]
        public virtual async Task DeleteAllAsync(GetStoryLoversInput input)
        {
            await _storyLoverRepository.DeleteAllAsync(input.FilterText, input.UserProfileId, input.StoryId);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new StoryLoverDownloadTokenCacheItem { Token = token },
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