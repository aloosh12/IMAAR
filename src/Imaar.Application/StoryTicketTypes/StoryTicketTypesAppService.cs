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
using Imaar.StoryTicketTypes;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.StoryTicketTypes
{

    [Authorize(ImaarPermissions.StoryTicketTypes.Default)]
    public abstract class StoryTicketTypesAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<StoryTicketTypeDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IStoryTicketTypeRepository _storyTicketTypeRepository;
        protected StoryTicketTypeManager _storyTicketTypeManager;

        public StoryTicketTypesAppServiceBase(IStoryTicketTypeRepository storyTicketTypeRepository, StoryTicketTypeManager storyTicketTypeManager, IDistributedCache<StoryTicketTypeDownloadTokenCacheItem, string> downloadTokenCache)
        {
            _downloadTokenCache = downloadTokenCache;
            _storyTicketTypeRepository = storyTicketTypeRepository;
            _storyTicketTypeManager = storyTicketTypeManager;

        }

        public virtual async Task<PagedResultDto<StoryTicketTypeDto>> GetListAsync(GetStoryTicketTypesInput input)
        {
            var totalCount = await _storyTicketTypeRepository.GetCountAsync(input.FilterText, input.Title, input.OrderMin, input.OrderMax, input.IsActive);
            var items = await _storyTicketTypeRepository.GetListAsync(input.FilterText, input.Title, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<StoryTicketTypeDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<StoryTicketType>, List<StoryTicketTypeDto>>(items)
            };
        }

        public virtual async Task<StoryTicketTypeDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<StoryTicketType, StoryTicketTypeDto>(await _storyTicketTypeRepository.GetAsync(id));
        }

        [Authorize(ImaarPermissions.StoryTicketTypes.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _storyTicketTypeRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.StoryTicketTypes.Create)]
        public virtual async Task<StoryTicketTypeDto> CreateAsync(StoryTicketTypeCreateDto input)
        {

            var storyTicketType = await _storyTicketTypeManager.CreateAsync(
            input.Title, input.Order, input.IsActive
            );

            return ObjectMapper.Map<StoryTicketType, StoryTicketTypeDto>(storyTicketType);
        }

        [Authorize(ImaarPermissions.StoryTicketTypes.Edit)]
        public virtual async Task<StoryTicketTypeDto> UpdateAsync(Guid id, StoryTicketTypeUpdateDto input)
        {

            var storyTicketType = await _storyTicketTypeManager.UpdateAsync(
            id,
            input.Title, input.Order, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<StoryTicketType, StoryTicketTypeDto>(storyTicketType);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(StoryTicketTypeExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _storyTicketTypeRepository.GetListAsync(input.FilterText, input.Title, input.OrderMin, input.OrderMax, input.IsActive);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<StoryTicketType>, List<StoryTicketTypeExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "StoryTicketTypes.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.StoryTicketTypes.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> storytickettypeIds)
        {
            await _storyTicketTypeRepository.DeleteManyAsync(storytickettypeIds);
        }

        [Authorize(ImaarPermissions.StoryTicketTypes.Delete)]
        public virtual async Task DeleteAllAsync(GetStoryTicketTypesInput input)
        {
            await _storyTicketTypeRepository.DeleteAllAsync(input.FilterText, input.Title, input.OrderMin, input.OrderMax, input.IsActive);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new StoryTicketTypeDownloadTokenCacheItem { Token = token },
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