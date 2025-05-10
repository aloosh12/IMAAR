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
using Imaar.TicketTypes;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.TicketTypes
{

    [Authorize(ImaarPermissions.TicketTypes.Default)]
    public abstract class TicketTypesAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<TicketTypeDownloadTokenCacheItem, string> _downloadTokenCache;
        protected ITicketTypeRepository _ticketTypeRepository;
        protected TicketTypeManager _ticketTypeManager;

        public TicketTypesAppServiceBase(ITicketTypeRepository ticketTypeRepository, TicketTypeManager ticketTypeManager, IDistributedCache<TicketTypeDownloadTokenCacheItem, string> downloadTokenCache)
        {
            _downloadTokenCache = downloadTokenCache;
            _ticketTypeRepository = ticketTypeRepository;
            _ticketTypeManager = ticketTypeManager;

        }

        public virtual async Task<PagedResultDto<TicketTypeDto>> GetListAsync(GetTicketTypesInput input)
        {
            var totalCount = await _ticketTypeRepository.GetCountAsync(input.FilterText, input.Title, input.OrderMin, input.OrderMax, input.IsActive);
            var items = await _ticketTypeRepository.GetListAsync(input.FilterText, input.Title, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<TicketTypeDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<TicketType>, List<TicketTypeDto>>(items)
            };
        }

        public virtual async Task<TicketTypeDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<TicketType, TicketTypeDto>(await _ticketTypeRepository.GetAsync(id));
        }

        [Authorize(ImaarPermissions.TicketTypes.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _ticketTypeRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.TicketTypes.Create)]
        public virtual async Task<TicketTypeDto> CreateAsync(TicketTypeCreateDto input)
        {

            var ticketType = await _ticketTypeManager.CreateAsync(
            input.Title, input.Order, input.IsActive
            );

            return ObjectMapper.Map<TicketType, TicketTypeDto>(ticketType);
        }

        [Authorize(ImaarPermissions.TicketTypes.Edit)]
        public virtual async Task<TicketTypeDto> UpdateAsync(Guid id, TicketTypeUpdateDto input)
        {

            var ticketType = await _ticketTypeManager.UpdateAsync(
            id,
            input.Title, input.Order, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<TicketType, TicketTypeDto>(ticketType);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(TicketTypeExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _ticketTypeRepository.GetListAsync(input.FilterText, input.Title, input.OrderMin, input.OrderMax, input.IsActive);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<TicketType>, List<TicketTypeExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "TicketTypes.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.TicketTypes.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> tickettypeIds)
        {
            await _ticketTypeRepository.DeleteManyAsync(tickettypeIds);
        }

        [Authorize(ImaarPermissions.TicketTypes.Delete)]
        public virtual async Task DeleteAllAsync(GetTicketTypesInput input)
        {
            await _ticketTypeRepository.DeleteAllAsync(input.FilterText, input.Title, input.OrderMin, input.OrderMax, input.IsActive);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new TicketTypeDownloadTokenCacheItem { Token = token },
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