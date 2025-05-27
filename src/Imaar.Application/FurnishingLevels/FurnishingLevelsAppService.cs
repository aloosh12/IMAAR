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
using Imaar.FurnishingLevels;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.FurnishingLevels
{

    [Authorize(ImaarPermissions.FurnishingLevels.Default)]
    public abstract class FurnishingLevelsAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<FurnishingLevelDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IFurnishingLevelRepository _furnishingLevelRepository;
        protected FurnishingLevelManager _furnishingLevelManager;

        public FurnishingLevelsAppServiceBase(IFurnishingLevelRepository furnishingLevelRepository, FurnishingLevelManager furnishingLevelManager, IDistributedCache<FurnishingLevelDownloadTokenCacheItem, string> downloadTokenCache)
        {
            _downloadTokenCache = downloadTokenCache;
            _furnishingLevelRepository = furnishingLevelRepository;
            _furnishingLevelManager = furnishingLevelManager;

        }

        public virtual async Task<PagedResultDto<FurnishingLevelDto>> GetListAsync(GetFurnishingLevelsInput input)
        {
            var totalCount = await _furnishingLevelRepository.GetCountAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive);
            var items = await _furnishingLevelRepository.GetListAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<FurnishingLevelDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<FurnishingLevel>, List<FurnishingLevelDto>>(items)
            };
        }

        public virtual async Task<FurnishingLevelDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<FurnishingLevel, FurnishingLevelDto>(await _furnishingLevelRepository.GetAsync(id));
        }

        [Authorize(ImaarPermissions.FurnishingLevels.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _furnishingLevelRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.FurnishingLevels.Create)]
        public virtual async Task<FurnishingLevelDto> CreateAsync(FurnishingLevelCreateDto input)
        {

            var furnishingLevel = await _furnishingLevelManager.CreateAsync(
            input.Name, input.IsActive, input.Order
            );

            return ObjectMapper.Map<FurnishingLevel, FurnishingLevelDto>(furnishingLevel);
        }

        [Authorize(ImaarPermissions.FurnishingLevels.Edit)]
        public virtual async Task<FurnishingLevelDto> UpdateAsync(Guid id, FurnishingLevelUpdateDto input)
        {

            var furnishingLevel = await _furnishingLevelManager.UpdateAsync(
            id,
            input.Name, input.IsActive, input.Order, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<FurnishingLevel, FurnishingLevelDto>(furnishingLevel);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(FurnishingLevelExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _furnishingLevelRepository.GetListAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<FurnishingLevel>, List<FurnishingLevelExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "FurnishingLevels.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.FurnishingLevels.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> furnishinglevelIds)
        {
            await _furnishingLevelRepository.DeleteManyAsync(furnishinglevelIds);
        }

        [Authorize(ImaarPermissions.FurnishingLevels.Delete)]
        public virtual async Task DeleteAllAsync(GetFurnishingLevelsInput input)
        {
            await _furnishingLevelRepository.DeleteAllAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new FurnishingLevelDownloadTokenCacheItem { Token = token },
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