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
using Imaar.ServiceTicketTypes;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.ServiceTicketTypes
{

    [Authorize(ImaarPermissions.ServiceTicketTypes.Default)]
    public abstract class ServiceTicketTypesAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<ServiceTicketTypeDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IServiceTicketTypeRepository _serviceTicketTypeRepository;
        protected ServiceTicketTypeManager _serviceTicketTypeManager;

        public ServiceTicketTypesAppServiceBase(IServiceTicketTypeRepository serviceTicketTypeRepository, ServiceTicketTypeManager serviceTicketTypeManager, IDistributedCache<ServiceTicketTypeDownloadTokenCacheItem, string> downloadTokenCache)
        {
            _downloadTokenCache = downloadTokenCache;
            _serviceTicketTypeRepository = serviceTicketTypeRepository;
            _serviceTicketTypeManager = serviceTicketTypeManager;

        }

        public virtual async Task<PagedResultDto<ServiceTicketTypeDto>> GetListAsync(GetServiceTicketTypesInput input)
        {
            var totalCount = await _serviceTicketTypeRepository.GetCountAsync(input.FilterText, input.Title, input.OrderMin, input.OrderMax, input.IsActive);
            var items = await _serviceTicketTypeRepository.GetListAsync(input.FilterText, input.Title, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<ServiceTicketTypeDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<ServiceTicketType>, List<ServiceTicketTypeDto>>(items)
            };
        }

        public virtual async Task<ServiceTicketTypeDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<ServiceTicketType, ServiceTicketTypeDto>(await _serviceTicketTypeRepository.GetAsync(id));
        }

        [Authorize(ImaarPermissions.ServiceTicketTypes.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _serviceTicketTypeRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.ServiceTicketTypes.Create)]
        public virtual async Task<ServiceTicketTypeDto> CreateAsync(ServiceTicketTypeCreateDto input)
        {

            var serviceTicketType = await _serviceTicketTypeManager.CreateAsync(
            input.Title, input.IsActive, input.Order
            );

            return ObjectMapper.Map<ServiceTicketType, ServiceTicketTypeDto>(serviceTicketType);
        }

        [Authorize(ImaarPermissions.ServiceTicketTypes.Edit)]
        public virtual async Task<ServiceTicketTypeDto> UpdateAsync(Guid id, ServiceTicketTypeUpdateDto input)
        {

            var serviceTicketType = await _serviceTicketTypeManager.UpdateAsync(
            id,
            input.Title, input.IsActive, input.Order, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<ServiceTicketType, ServiceTicketTypeDto>(serviceTicketType);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(ServiceTicketTypeExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _serviceTicketTypeRepository.GetListAsync(input.FilterText, input.Title, input.OrderMin, input.OrderMax, input.IsActive);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<ServiceTicketType>, List<ServiceTicketTypeExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "ServiceTicketTypes.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.ServiceTicketTypes.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> servicetickettypeIds)
        {
            await _serviceTicketTypeRepository.DeleteManyAsync(servicetickettypeIds);
        }

        [Authorize(ImaarPermissions.ServiceTicketTypes.Delete)]
        public virtual async Task DeleteAllAsync(GetServiceTicketTypesInput input)
        {
            await _serviceTicketTypeRepository.DeleteAllAsync(input.FilterText, input.Title, input.OrderMin, input.OrderMax, input.IsActive);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new ServiceTicketTypeDownloadTokenCacheItem { Token = token },
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