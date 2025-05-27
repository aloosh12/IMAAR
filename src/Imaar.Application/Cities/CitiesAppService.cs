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
using Imaar.Cities;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.Cities
{

    [Authorize(ImaarPermissions.Cities.Default)]
    public abstract class CitiesAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<CityDownloadTokenCacheItem, string> _downloadTokenCache;
        protected ICityRepository _cityRepository;
        protected CityManager _cityManager;

        public CitiesAppServiceBase(ICityRepository cityRepository, CityManager cityManager, IDistributedCache<CityDownloadTokenCacheItem, string> downloadTokenCache)
        {
            _downloadTokenCache = downloadTokenCache;
            _cityRepository = cityRepository;
            _cityManager = cityManager;

        }

        public virtual async Task<PagedResultDto<CityDto>> GetListAsync(GetCitiesInput input)
        {
            var totalCount = await _cityRepository.GetCountAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive);
            var items = await _cityRepository.GetListAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<CityDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<City>, List<CityDto>>(items)
            };
        }

        public virtual async Task<CityDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<City, CityDto>(await _cityRepository.GetAsync(id));
        }

        [Authorize(ImaarPermissions.Cities.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _cityRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.Cities.Create)]
        public virtual async Task<CityDto> CreateAsync(CityCreateDto input)
        {

            var city = await _cityManager.CreateAsync(
            input.Name, input.IsActive, input.Order
            );

            return ObjectMapper.Map<City, CityDto>(city);
        }

        [Authorize(ImaarPermissions.Cities.Edit)]
        public virtual async Task<CityDto> UpdateAsync(Guid id, CityUpdateDto input)
        {

            var city = await _cityManager.UpdateAsync(
            id,
            input.Name, input.IsActive, input.Order, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<City, CityDto>(city);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(CityExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _cityRepository.GetListAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<City>, List<CityExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Cities.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.Cities.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> cityIds)
        {
            await _cityRepository.DeleteManyAsync(cityIds);
        }

        [Authorize(ImaarPermissions.Cities.Delete)]
        public virtual async Task DeleteAllAsync(GetCitiesInput input)
        {
            await _cityRepository.DeleteAllAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new CityDownloadTokenCacheItem { Token = token },
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