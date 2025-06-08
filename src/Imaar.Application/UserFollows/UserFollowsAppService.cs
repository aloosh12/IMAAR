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
using Imaar.UserFollows;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.UserFollows
{

    [Authorize(ImaarPermissions.UserFollows.Default)]
    public abstract class UserFollowsAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<UserFollowDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IUserFollowRepository _userFollowRepository;
        protected UserFollowManager _userFollowManager;

        protected IRepository<Imaar.UserProfiles.UserProfile, Guid> _userProfileRepository;

        public UserFollowsAppServiceBase(IUserFollowRepository userFollowRepository, UserFollowManager userFollowManager, IDistributedCache<UserFollowDownloadTokenCacheItem, string> downloadTokenCache, IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _userFollowRepository = userFollowRepository;
            _userFollowManager = userFollowManager; _userProfileRepository = userProfileRepository;

        }

        public virtual async Task<PagedResultDto<UserFollowWithNavigationPropertiesDto>> GetListAsync(GetUserFollowsInput input)
        {
            var totalCount = await _userFollowRepository.GetCountAsync(input.FilterText, input.FollowerUserId, input.FollowingUserId);
            var items = await _userFollowRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.FollowerUserId, input.FollowingUserId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<UserFollowWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<UserFollowWithNavigationProperties>, List<UserFollowWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<UserFollowWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<UserFollowWithNavigationProperties, UserFollowWithNavigationPropertiesDto>
                (await _userFollowRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<UserFollowDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<UserFollow, UserFollowDto>(await _userFollowRepository.GetAsync(id));
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

        [Authorize(ImaarPermissions.UserFollows.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _userFollowRepository.DeleteAsync(id);
        }
        
        public virtual async Task<bool> UserFollowExistsAsync(Guid followerUserId, Guid followingUserId)
        {
            // Check if a user follow record already exists
            var query = await GetListAsync(new GetUserFollowsInput
            {
                FollowerUserId = followerUserId,
                FollowingUserId = followingUserId,
                MaxResultCount = 1
            });
            
            return query.TotalCount > 0;
        }

        [Authorize(ImaarPermissions.UserFollows.Create)]
        public virtual async Task<UserFollowDto> CreateAsync(UserFollowCreateDto input)
        {
            if (input.FollowerUserId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.FollowingUserId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            
            // Check if this user already follows the target user
            if (await UserFollowExistsAsync(input.FollowerUserId, input.FollowingUserId))
            {
                throw new UserFriendlyException("You already follow this user");
            }

            var userFollow = await _userFollowManager.CreateAsync(
            input.FollowerUserId, input.FollowingUserId
            );

            return ObjectMapper.Map<UserFollow, UserFollowDto>(userFollow);
        }

        [Authorize(ImaarPermissions.UserFollows.Edit)]
        public virtual async Task<UserFollowDto> UpdateAsync(Guid id, UserFollowUpdateDto input)
        {
            if (input.FollowerUserId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.FollowingUserId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }

            var userFollow = await _userFollowManager.UpdateAsync(
            id,
            input.FollowerUserId, input.FollowingUserId, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<UserFollow, UserFollowDto>(userFollow);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(UserFollowExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var userFollows = await _userFollowRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.FollowerUserId, input.FollowingUserId);
            var items = userFollows.Select(item => new
            {

                FollowerUser = item.FollowerUser?.SecurityNumber,
                FollowingUser = item.FollowingUser?.SecurityNumber,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "UserFollows.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.UserFollows.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> userfollowIds)
        {
            await _userFollowRepository.DeleteManyAsync(userfollowIds);
        }

        [Authorize(ImaarPermissions.UserFollows.Delete)]
        public virtual async Task DeleteAllAsync(GetUserFollowsInput input)
        {
            await _userFollowRepository.DeleteAllAsync(input.FilterText, input.FollowerUserId, input.FollowingUserId);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new UserFollowDownloadTokenCacheItem { Token = token },
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