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
using Imaar.Advertisements;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.Advertisements
{

    [Authorize(ImaarPermissions.Advertisements.Default)]
    public abstract class AdvertisementsAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<AdvertisementDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IAdvertisementRepository _advertisementRepository;
        protected AdvertisementManager _advertisementManager;

        protected IRepository<Imaar.UserProfiles.UserProfile, Guid> _userProfileRepository;

        public AdvertisementsAppServiceBase(IAdvertisementRepository advertisementRepository, AdvertisementManager advertisementManager, IDistributedCache<AdvertisementDownloadTokenCacheItem, string> downloadTokenCache, IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _advertisementRepository = advertisementRepository;
            _advertisementManager = advertisementManager; _userProfileRepository = userProfileRepository;

        }

        public virtual async Task<PagedResultDto<AdvertisementWithNavigationPropertiesDto>> GetListAsync(GetAdvertisementsInput input)
        {
            var totalCount = await _advertisementRepository.GetCountAsync(input.FilterText, input.Title, input.SubTitle, input.File, input.FromDateTimeMin, input.FromDateTimeMax, input.ToDateTimeMin, input.ToDateTimeMax, input.OrderMin, input.OrderMax, input.IsActive, input.UserProfileId);
            var items = await _advertisementRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Title, input.SubTitle, input.File, input.FromDateTimeMin, input.FromDateTimeMax, input.ToDateTimeMin, input.ToDateTimeMax, input.OrderMin, input.OrderMax, input.IsActive, input.UserProfileId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<AdvertisementWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<AdvertisementWithNavigationProperties>, List<AdvertisementWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<AdvertisementWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<AdvertisementWithNavigationProperties, AdvertisementWithNavigationPropertiesDto>
                (await _advertisementRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<AdvertisementDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Advertisement, AdvertisementDto>(await _advertisementRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input)
        {
            var query = (await _userProfileRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Email != null &&
                         x.Email.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.UserProfiles.UserProfile>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.UserProfiles.UserProfile>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(ImaarPermissions.Advertisements.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _advertisementRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.Advertisements.Create)]
        public virtual async Task<AdvertisementDto> CreateAsync(AdvertisementCreateDto input)
        {

            var advertisement = await _advertisementManager.CreateAsync(
            input.UserProfileId, input.File, input.Order, input.IsActive, input.Title, input.SubTitle, input.FromDateTime, input.ToDateTime
            );

            return ObjectMapper.Map<Advertisement, AdvertisementDto>(advertisement);
        }

        [Authorize(ImaarPermissions.Advertisements.Edit)]
        public virtual async Task<AdvertisementDto> UpdateAsync(Guid id, AdvertisementUpdateDto input)
        {

            var advertisement = await _advertisementManager.UpdateAsync(
            id,
            input.UserProfileId, input.File, input.Order, input.IsActive, input.Title, input.SubTitle, input.FromDateTime, input.ToDateTime, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Advertisement, AdvertisementDto>(advertisement);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(AdvertisementExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var advertisements = await _advertisementRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Title, input.SubTitle, input.File, input.FromDateTimeMin, input.FromDateTimeMax, input.ToDateTimeMin, input.ToDateTimeMax, input.OrderMin, input.OrderMax, input.IsActive, input.UserProfileId);
            var items = advertisements.Select(item => new
            {
                Title = item.Advertisement.Title,
                SubTitle = item.Advertisement.SubTitle,
                File = item.Advertisement.File,
                FromDateTime = item.Advertisement.FromDateTime,
                ToDateTime = item.Advertisement.ToDateTime,
                Order = item.Advertisement.Order,
                IsActive = item.Advertisement.IsActive,

                UserProfile = item.UserProfile?.Email,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Advertisements.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.Advertisements.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> advertisementIds)
        {
            await _advertisementRepository.DeleteManyAsync(advertisementIds);
        }

        [Authorize(ImaarPermissions.Advertisements.Delete)]
        public virtual async Task DeleteAllAsync(GetAdvertisementsInput input)
        {
            await _advertisementRepository.DeleteAllAsync(input.FilterText, input.Title, input.SubTitle, input.File, input.FromDateTimeMin, input.FromDateTimeMax, input.ToDateTimeMin, input.ToDateTimeMax, input.OrderMin, input.OrderMax, input.IsActive, input.UserProfileId);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new AdvertisementDownloadTokenCacheItem { Token = token },
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