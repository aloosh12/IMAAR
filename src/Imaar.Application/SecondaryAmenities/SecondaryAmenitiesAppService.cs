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
using Imaar.SecondaryAmenities;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.SecondaryAmenities
{

    [Authorize(ImaarPermissions.SecondaryAmenities.Default)]
    public abstract class SecondaryAmenitiesAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<SecondaryAmenityDownloadTokenCacheItem, string> _downloadTokenCache;
        protected ISecondaryAmenityRepository _secondaryAmenityRepository;
        protected SecondaryAmenityManager _secondaryAmenityManager;

        public SecondaryAmenitiesAppServiceBase(ISecondaryAmenityRepository secondaryAmenityRepository, SecondaryAmenityManager secondaryAmenityManager, IDistributedCache<SecondaryAmenityDownloadTokenCacheItem, string> downloadTokenCache)
        {
            _downloadTokenCache = downloadTokenCache;
            _secondaryAmenityRepository = secondaryAmenityRepository;
            _secondaryAmenityManager = secondaryAmenityManager;

        }

        public virtual async Task<PagedResultDto<SecondaryAmenityDto>> GetListAsync(GetSecondaryAmenitiesInput input)
        {
            var totalCount = await _secondaryAmenityRepository.GetCountAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive);
            var items = await _secondaryAmenityRepository.GetListAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<SecondaryAmenityDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<SecondaryAmenity>, List<SecondaryAmenityDto>>(items)
            };
        }

        public virtual async Task<SecondaryAmenityDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<SecondaryAmenity, SecondaryAmenityDto>(await _secondaryAmenityRepository.GetAsync(id));
        }

        [Authorize(ImaarPermissions.SecondaryAmenities.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _secondaryAmenityRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.SecondaryAmenities.Create)]
        public virtual async Task<SecondaryAmenityDto> CreateAsync(SecondaryAmenityCreateDto input)
        {

            var secondaryAmenity = await _secondaryAmenityManager.CreateAsync(
            input.Name, input.IsActive, input.Order
            );

            return ObjectMapper.Map<SecondaryAmenity, SecondaryAmenityDto>(secondaryAmenity);
        }

        [Authorize(ImaarPermissions.SecondaryAmenities.Edit)]
        public virtual async Task<SecondaryAmenityDto> UpdateAsync(Guid id, SecondaryAmenityUpdateDto input)
        {

            var secondaryAmenity = await _secondaryAmenityManager.UpdateAsync(
            id,
            input.Name, input.IsActive, input.Order, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<SecondaryAmenity, SecondaryAmenityDto>(secondaryAmenity);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(SecondaryAmenityExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _secondaryAmenityRepository.GetListAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<SecondaryAmenity>, List<SecondaryAmenityExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "SecondaryAmenities.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.SecondaryAmenities.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> secondaryamenityIds)
        {
            await _secondaryAmenityRepository.DeleteManyAsync(secondaryamenityIds);
        }

        [Authorize(ImaarPermissions.SecondaryAmenities.Delete)]
        public virtual async Task DeleteAllAsync(GetSecondaryAmenitiesInput input)
        {
            await _secondaryAmenityRepository.DeleteAllAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new SecondaryAmenityDownloadTokenCacheItem { Token = token },
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