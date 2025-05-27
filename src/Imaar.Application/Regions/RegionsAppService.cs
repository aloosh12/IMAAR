using Imaar.Shared;
using Imaar.Cities;
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
using Imaar.Regions;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.Regions
{

    [Authorize(ImaarPermissions.Regions.Default)]
    public abstract class RegionsAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<RegionDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IRegionRepository _regionRepository;
        protected RegionManager _regionManager;

        protected IRepository<Imaar.Cities.City, Guid> _cityRepository;

        public RegionsAppServiceBase(IRegionRepository regionRepository, RegionManager regionManager, IDistributedCache<RegionDownloadTokenCacheItem, string> downloadTokenCache, IRepository<Imaar.Cities.City, Guid> cityRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _regionRepository = regionRepository;
            _regionManager = regionManager; _cityRepository = cityRepository;

        }

        public virtual async Task<PagedResultDto<RegionWithNavigationPropertiesDto>> GetListAsync(GetRegionsInput input)
        {
            var totalCount = await _regionRepository.GetCountAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive, input.CityId);
            var items = await _regionRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive, input.CityId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<RegionWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<RegionWithNavigationProperties>, List<RegionWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<RegionWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<RegionWithNavigationProperties, RegionWithNavigationPropertiesDto>
                (await _regionRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<RegionDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Region, RegionDto>(await _regionRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetCityLookupAsync(LookupRequestDto input)
        {
            var query = (await _cityRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null &&
                         x.Name.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.Cities.City>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.Cities.City>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(ImaarPermissions.Regions.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _regionRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.Regions.Create)]
        public virtual async Task<RegionDto> CreateAsync(RegionCreateDto input)
        {
            if (input.CityId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["City"]]);
            }

            var region = await _regionManager.CreateAsync(
            input.CityId, input.Name, input.IsActive, input.Order
            );

            return ObjectMapper.Map<Region, RegionDto>(region);
        }

        [Authorize(ImaarPermissions.Regions.Edit)]
        public virtual async Task<RegionDto> UpdateAsync(Guid id, RegionUpdateDto input)
        {
            if (input.CityId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["City"]]);
            }

            var region = await _regionManager.UpdateAsync(
            id,
            input.CityId, input.Name, input.IsActive, input.Order, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Region, RegionDto>(region);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(RegionExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var regions = await _regionRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive, input.CityId);
            var items = regions.Select(item => new
            {
                Name = item.Region.Name,
                Order = item.Region.Order,
                IsActive = item.Region.IsActive,

                City = item.City?.Name,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Regions.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.Regions.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> regionIds)
        {
            await _regionRepository.DeleteManyAsync(regionIds);
        }

        [Authorize(ImaarPermissions.Regions.Delete)]
        public virtual async Task DeleteAllAsync(GetRegionsInput input)
        {
            await _regionRepository.DeleteAllAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive, input.CityId);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new RegionDownloadTokenCacheItem { Token = token },
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