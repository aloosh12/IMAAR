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
using Imaar.UserSavedItems;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.UserSavedItems
{

    [Authorize(ImaarPermissions.UserSavedItems.Default)]
    public abstract class UserSavedItemsAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<UserSavedItemDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IUserSavedItemRepository _userSavedItemRepository;
        protected UserSavedItemManager _userSavedItemManager;

        protected IRepository<Imaar.UserProfiles.UserProfile, Guid> _userProfileRepository;

        public UserSavedItemsAppServiceBase(IUserSavedItemRepository userSavedItemRepository, UserSavedItemManager userSavedItemManager, IDistributedCache<UserSavedItemDownloadTokenCacheItem, string> downloadTokenCache, IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _userSavedItemRepository = userSavedItemRepository;
            _userSavedItemManager = userSavedItemManager; _userProfileRepository = userProfileRepository;

        }

        public virtual async Task<PagedResultDto<UserSavedItemWithNavigationPropertiesDto>> GetListAsync(GetUserSavedItemsInput input)
        {
            var totalCount = await _userSavedItemRepository.GetCountAsync(input.FilterText, input.SourceId, input.SavedItemType, input.UserProfileId);
            var items = await _userSavedItemRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.SourceId, input.SavedItemType, input.UserProfileId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<UserSavedItemWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<UserSavedItemWithNavigationProperties>, List<UserSavedItemWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<UserSavedItemWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<UserSavedItemWithNavigationProperties, UserSavedItemWithNavigationPropertiesDto>
                (await _userSavedItemRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<UserSavedItemDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<UserSavedItem, UserSavedItemDto>(await _userSavedItemRepository.GetAsync(id));
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

        [Authorize(ImaarPermissions.UserSavedItems.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _userSavedItemRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.UserSavedItems.Create)]
        public virtual async Task<UserSavedItemDto> CreateAsync(UserSavedItemCreateDto input)
        {
            if (input.UserProfileId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }

            var userSavedItem = await _userSavedItemManager.CreateAsync(
            input.UserProfileId, input.SourceId, input.SavedItemType
            );

            return ObjectMapper.Map<UserSavedItem, UserSavedItemDto>(userSavedItem);
        }

        [Authorize(ImaarPermissions.UserSavedItems.Edit)]
        public virtual async Task<UserSavedItemDto> UpdateAsync(Guid id, UserSavedItemUpdateDto input)
        {
            if (input.UserProfileId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }

            var userSavedItem = await _userSavedItemManager.UpdateAsync(
            id,
            input.UserProfileId, input.SourceId, input.SavedItemType, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<UserSavedItem, UserSavedItemDto>(userSavedItem);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(UserSavedItemExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var userSavedItems = await _userSavedItemRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.SourceId, input.SavedItemType, input.UserProfileId);
            var items = userSavedItems.Select(item => new
            {
                SourceId = item.UserSavedItem.SourceId,
                SavedItemType = item.UserSavedItem.SavedItemType,

                UserProfile = item.UserProfile?.SecurityNumber,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "UserSavedItems.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.UserSavedItems.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> usersaveditemIds)
        {
            await _userSavedItemRepository.DeleteManyAsync(usersaveditemIds);
        }

        [Authorize(ImaarPermissions.UserSavedItems.Delete)]
        public virtual async Task DeleteAllAsync(GetUserSavedItemsInput input)
        {
            await _userSavedItemRepository.DeleteAllAsync(input.FilterText, input.SourceId, input.SavedItemType, input.UserProfileId);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new UserSavedItemDownloadTokenCacheItem { Token = token },
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