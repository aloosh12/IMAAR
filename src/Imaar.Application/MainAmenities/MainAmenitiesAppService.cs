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
using Imaar.MainAmenities;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.MainAmenities
{

    [Authorize(ImaarPermissions.MainAmenities.Default)]
    public abstract class MainAmenitiesAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<MainAmenityDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IMainAmenityRepository _mainAmenityRepository;
        protected MainAmenityManager _mainAmenityManager;

        public MainAmenitiesAppServiceBase(IMainAmenityRepository mainAmenityRepository, MainAmenityManager mainAmenityManager, IDistributedCache<MainAmenityDownloadTokenCacheItem, string> downloadTokenCache)
        {
            _downloadTokenCache = downloadTokenCache;
            _mainAmenityRepository = mainAmenityRepository;
            _mainAmenityManager = mainAmenityManager;

        }

        public virtual async Task<PagedResultDto<MainAmenityDto>> GetListAsync(GetMainAmenitiesInput input)
        {
            var totalCount = await _mainAmenityRepository.GetCountAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive);
            var items = await _mainAmenityRepository.GetListAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<MainAmenityDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<MainAmenity>, List<MainAmenityDto>>(items)
            };
        }

        public virtual async Task<MainAmenityDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<MainAmenity, MainAmenityDto>(await _mainAmenityRepository.GetAsync(id));
        }

        [Authorize(ImaarPermissions.MainAmenities.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _mainAmenityRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.MainAmenities.Create)]
        public virtual async Task<MainAmenityDto> CreateAsync(MainAmenityCreateDto input)
        {

            var mainAmenity = await _mainAmenityManager.CreateAsync(
            input.Name, input.IsActive, input.Order
            );

            return ObjectMapper.Map<MainAmenity, MainAmenityDto>(mainAmenity);
        }

        [Authorize(ImaarPermissions.MainAmenities.Edit)]
        public virtual async Task<MainAmenityDto> UpdateAsync(Guid id, MainAmenityUpdateDto input)
        {

            var mainAmenity = await _mainAmenityManager.UpdateAsync(
            id,
            input.Name, input.IsActive, input.Order, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<MainAmenity, MainAmenityDto>(mainAmenity);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(MainAmenityExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _mainAmenityRepository.GetListAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<MainAmenity>, List<MainAmenityExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "MainAmenities.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.MainAmenities.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> mainamenityIds)
        {
            await _mainAmenityRepository.DeleteManyAsync(mainamenityIds);
        }

        [Authorize(ImaarPermissions.MainAmenities.Delete)]
        public virtual async Task DeleteAllAsync(GetMainAmenitiesInput input)
        {
            await _mainAmenityRepository.DeleteAllAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new MainAmenityDownloadTokenCacheItem { Token = token },
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