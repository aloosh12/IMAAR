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
using Imaar.UserWorksExhibitions;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.UserWorksExhibitions
{

    [Authorize(ImaarPermissions.UserWorksExhibitions.Default)]
    public abstract class UserWorksExhibitionsAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<UserWorksExhibitionDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IUserWorksExhibitionRepository _userWorksExhibitionRepository;
        protected UserWorksExhibitionManager _userWorksExhibitionManager;

        protected IRepository<Imaar.UserProfiles.UserProfile, Guid> _userProfileRepository;

        public UserWorksExhibitionsAppServiceBase(IUserWorksExhibitionRepository userWorksExhibitionRepository, UserWorksExhibitionManager userWorksExhibitionManager, IDistributedCache<UserWorksExhibitionDownloadTokenCacheItem, string> downloadTokenCache, IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _userWorksExhibitionRepository = userWorksExhibitionRepository;
            _userWorksExhibitionManager = userWorksExhibitionManager; _userProfileRepository = userProfileRepository;

        }

        public virtual async Task<PagedResultDto<UserWorksExhibitionWithNavigationPropertiesDto>> GetListAsync(GetUserWorksExhibitionsInput input)
        {
            var totalCount = await _userWorksExhibitionRepository.GetCountAsync(input.FilterText, input.Title, input.File, input.OrderMin, input.OrderMax, input.UserProfileId);
            var items = await _userWorksExhibitionRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Title, input.File, input.OrderMin, input.OrderMax, input.UserProfileId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<UserWorksExhibitionWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<UserWorksExhibitionWithNavigationProperties>, List<UserWorksExhibitionWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<UserWorksExhibitionWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<UserWorksExhibitionWithNavigationProperties, UserWorksExhibitionWithNavigationPropertiesDto>
                (await _userWorksExhibitionRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<UserWorksExhibitionDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<UserWorksExhibition, UserWorksExhibitionDto>(await _userWorksExhibitionRepository.GetAsync(id));
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

        [Authorize(ImaarPermissions.UserWorksExhibitions.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _userWorksExhibitionRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.UserWorksExhibitions.Create)]
        public virtual async Task<UserWorksExhibitionDto> CreateAsync(UserWorksExhibitionCreateDto input)
        {
            if (input.UserProfileId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }

            var userWorksExhibition = await _userWorksExhibitionManager.CreateAsync(
            input.UserProfileId, input.File, input.Order, input.Title
            );

            return ObjectMapper.Map<UserWorksExhibition, UserWorksExhibitionDto>(userWorksExhibition);
        }

        [Authorize(ImaarPermissions.UserWorksExhibitions.Edit)]
        public virtual async Task<UserWorksExhibitionDto> UpdateAsync(Guid id, UserWorksExhibitionUpdateDto input)
        {
            if (input.UserProfileId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }

            var userWorksExhibition = await _userWorksExhibitionManager.UpdateAsync(
            id,
            input.UserProfileId, input.File, input.Order, input.Title, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<UserWorksExhibition, UserWorksExhibitionDto>(userWorksExhibition);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(UserWorksExhibitionExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var userWorksExhibitions = await _userWorksExhibitionRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Title, input.File, input.OrderMin, input.OrderMax, input.UserProfileId);
            var items = userWorksExhibitions.Select(item => new
            {
                Title = item.UserWorksExhibition.Title,
                File = item.UserWorksExhibition.File,
                Order = item.UserWorksExhibition.Order,

                UserProfile = item.UserProfile?.SecurityNumber,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "UserWorksExhibitions.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.UserWorksExhibitions.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> userworksexhibitionIds)
        {
            await _userWorksExhibitionRepository.DeleteManyAsync(userworksexhibitionIds);
        }

        [Authorize(ImaarPermissions.UserWorksExhibitions.Delete)]
        public virtual async Task DeleteAllAsync(GetUserWorksExhibitionsInput input)
        {
            await _userWorksExhibitionRepository.DeleteAllAsync(input.FilterText, input.Title, input.File, input.OrderMin, input.OrderMax, input.UserProfileId);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new UserWorksExhibitionDownloadTokenCacheItem { Token = token },
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