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
using Imaar.Medias;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;
using AutoMapper.Internal.Mappers;
using Imaar.Medias;

namespace Imaar.Medias
{

    [Authorize(ImaarPermissions.Medias.Default)]
    public abstract class MediasAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<MediaDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IMediaRepository _mediaRepository;
        protected MediaManager _mediaManager;

        public MediasAppServiceBase(IMediaRepository mediaRepository, MediaManager mediaManager, IDistributedCache<MediaDownloadTokenCacheItem, string> downloadTokenCache)
        {
            _downloadTokenCache = downloadTokenCache;
            _mediaRepository = mediaRepository;
            _mediaManager = mediaManager;

        }

        public virtual async Task<PagedResultDto<MediaDto>> GetListAsync(GetMediasInput input)
        {
            var totalCount = await _mediaRepository.GetCountAsync(input.FilterText, input.Title, input.File, input.OrderMin, input.OrderMax, input.IsActive, input.SourceEntityType, input.SourceEntityId);
            var items = await _mediaRepository.GetListAsync(input.FilterText, input.Title, input.File, input.OrderMin, input.OrderMax, input.IsActive, input.SourceEntityType, input.SourceEntityId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<MediaDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Media>, List<MediaDto>>(items)
            };
        }

        public virtual async Task<MediaDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Media, MediaDto>(await _mediaRepository.GetAsync(id));
        }

        [Authorize(ImaarPermissions.Medias.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _mediaRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.Medias.Create)]
        public virtual async Task<MediaDto> CreateAsync(MediaCreateDto input)
        {

            var media = await _mediaManager.CreateAsync(
            input.File, input.Order, input.IsActive, input.SourceEntityType, input.SourceEntityId, input.Title
            );

            return ObjectMapper.Map<Media, MediaDto>(media);
        }

        [Authorize(ImaarPermissions.Medias.Edit)]
        public virtual async Task<MediaDto> UpdateAsync(Guid id, MediaUpdateDto input)
        {

            var media = await _mediaManager.UpdateAsync(
            id,
            input.File, input.Order, input.IsActive, input.SourceEntityType, input.SourceEntityId, input.Title, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Media, MediaDto>(media);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(MediaExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _mediaRepository.GetListAsync(input.FilterText, input.Title, input.File, input.OrderMin, input.OrderMax, input.IsActive, input.SourceEntityType, input.SourceEntityId);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<Media>, List<MediaExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Medias.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.Medias.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> mediaIds)
        {
            await _mediaRepository.DeleteManyAsync(mediaIds);
        }

        [Authorize(ImaarPermissions.Medias.Delete)]
        public virtual async Task DeleteAllAsync(GetMediasInput input)
        {
            await _mediaRepository.DeleteAllAsync(input.FilterText, input.Title, input.File, input.OrderMin, input.OrderMax, input.IsActive, input.SourceEntityType, input.SourceEntityId);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new MediaDownloadTokenCacheItem { Token = token },
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