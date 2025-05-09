using Imaar.Shared;
using Imaar.UserProfiles;
using Imaar.ServiceTypes;
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
using Imaar.ImaarServices;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.ImaarServices
{

    [Authorize(ImaarPermissions.ImaarServices.Default)]
    public abstract class ImaarServicesAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<ImaarServiceDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IImaarServiceRepository _imaarServiceRepository;
        protected ImaarServiceManager _imaarServiceManager;

        protected IRepository<Imaar.ServiceTypes.ServiceType, Guid> _serviceTypeRepository;
        protected IRepository<Imaar.UserProfiles.UserProfile, Guid> _userProfileRepository;

        public ImaarServicesAppServiceBase(IImaarServiceRepository imaarServiceRepository, ImaarServiceManager imaarServiceManager, IDistributedCache<ImaarServiceDownloadTokenCacheItem, string> downloadTokenCache, IRepository<Imaar.ServiceTypes.ServiceType, Guid> serviceTypeRepository, IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _imaarServiceRepository = imaarServiceRepository;
            _imaarServiceManager = imaarServiceManager; _serviceTypeRepository = serviceTypeRepository;
            _userProfileRepository = userProfileRepository;

        }

        public virtual async Task<PagedResultDto<ImaarServiceWithNavigationPropertiesDto>> GetListAsync(GetImaarServicesInput input)
        {
            var totalCount = await _imaarServiceRepository.GetCountAsync(input.FilterText, input.Title, input.Description, input.ServiceLocation, input.ServiceNumber, input.DateOfPublishMin, input.DateOfPublishMax, input.PriceMin, input.PriceMax, input.Latitude, input.Longitude, input.ServiceTypeId, input.UserProfileId);
            var items = await _imaarServiceRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Title, input.Description, input.ServiceLocation, input.ServiceNumber, input.DateOfPublishMin, input.DateOfPublishMax, input.PriceMin, input.PriceMax, input.Latitude, input.Longitude, input.ServiceTypeId, input.UserProfileId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<ImaarServiceWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<ImaarServiceWithNavigationProperties>, List<ImaarServiceWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<ImaarServiceWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<ImaarServiceWithNavigationProperties, ImaarServiceWithNavigationPropertiesDto>
                (await _imaarServiceRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<ImaarServiceDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<ImaarService, ImaarServiceDto>(await _imaarServiceRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetServiceTypeLookupAsync(LookupRequestDto input)
        {
            var query = (await _serviceTypeRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Title != null &&
                         x.Title.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.ServiceTypes.ServiceType>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.ServiceTypes.ServiceType>, List<LookupDto<Guid>>>(lookupData)
            };
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

        [Authorize(ImaarPermissions.ImaarServices.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _imaarServiceRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.ImaarServices.Create)]
        public virtual async Task<ImaarServiceDto> CreateAsync(ImaarServiceCreateDto input)
        {
            if (input.ServiceTypeId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["ServiceType"]]);
            }
            if (input.UserProfileId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }

            var imaarService = await _imaarServiceManager.CreateAsync(
            input.ServiceTypeId, input.UserProfileId, input.Title, input.Description, input.ServiceLocation, input.ServiceNumber, input.DateOfPublish, input.Price, input.Latitude, input.Longitude
            );

            return ObjectMapper.Map<ImaarService, ImaarServiceDto>(imaarService);
        }

        [Authorize(ImaarPermissions.ImaarServices.Edit)]
        public virtual async Task<ImaarServiceDto> UpdateAsync(Guid id, ImaarServiceUpdateDto input)
        {
            if (input.ServiceTypeId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["ServiceType"]]);
            }
            if (input.UserProfileId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }

            var imaarService = await _imaarServiceManager.UpdateAsync(
            id,
            input.ServiceTypeId, input.UserProfileId, input.Title, input.Description, input.ServiceLocation, input.ServiceNumber, input.DateOfPublish, input.Price, input.Latitude, input.Longitude, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<ImaarService, ImaarServiceDto>(imaarService);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(ImaarServiceExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var imaarServices = await _imaarServiceRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Title, input.Description, input.ServiceLocation, input.ServiceNumber, input.DateOfPublishMin, input.DateOfPublishMax, input.PriceMin, input.PriceMax, input.Latitude, input.Longitude, input.ServiceTypeId, input.UserProfileId);
            var items = imaarServices.Select(item => new
            {
                Title = item.ImaarService.Title,
                Description = item.ImaarService.Description,
                ServiceLocation = item.ImaarService.ServiceLocation,
                ServiceNumber = item.ImaarService.ServiceNumber,
                DateOfPublish = item.ImaarService.DateOfPublish,
                Price = item.ImaarService.Price,
                Latitude = item.ImaarService.Latitude,
                Longitude = item.ImaarService.Longitude,

                ServiceType = item.ServiceType?.Title,
                UserProfile = item.UserProfile?.SecurityNumber,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "ImaarServices.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.ImaarServices.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> imaarserviceIds)
        {
            await _imaarServiceRepository.DeleteManyAsync(imaarserviceIds);
        }

        [Authorize(ImaarPermissions.ImaarServices.Delete)]
        public virtual async Task DeleteAllAsync(GetImaarServicesInput input)
        {
            await _imaarServiceRepository.DeleteAllAsync(input.FilterText, input.Title, input.Description, input.ServiceLocation, input.ServiceNumber, input.DateOfPublishMin, input.DateOfPublishMax, input.PriceMin, input.PriceMax, input.Latitude, input.Longitude, input.ServiceTypeId, input.UserProfileId);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new ImaarServiceDownloadTokenCacheItem { Token = token },
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