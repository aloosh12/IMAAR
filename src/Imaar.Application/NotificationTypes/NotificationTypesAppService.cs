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
using Imaar.NotificationTypes;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.NotificationTypes
{

    [Authorize(ImaarPermissions.NotificationTypes.Default)]
    public abstract class NotificationTypesAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<NotificationTypeDownloadTokenCacheItem, string> _downloadTokenCache;
        protected INotificationTypeRepository _notificationTypeRepository;
        protected NotificationTypeManager _notificationTypeManager;

        public NotificationTypesAppServiceBase(INotificationTypeRepository notificationTypeRepository, NotificationTypeManager notificationTypeManager, IDistributedCache<NotificationTypeDownloadTokenCacheItem, string> downloadTokenCache)
        {
            _downloadTokenCache = downloadTokenCache;
            _notificationTypeRepository = notificationTypeRepository;
            _notificationTypeManager = notificationTypeManager;

        }

        public virtual async Task<PagedResultDto<NotificationTypeDto>> GetListAsync(GetNotificationTypesInput input)
        {
            var totalCount = await _notificationTypeRepository.GetCountAsync(input.FilterText, input.Title);
            var items = await _notificationTypeRepository.GetListAsync(input.FilterText, input.Title, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<NotificationTypeDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<NotificationType>, List<NotificationTypeDto>>(items)
            };
        }

        public virtual async Task<NotificationTypeDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<NotificationType, NotificationTypeDto>(await _notificationTypeRepository.GetAsync(id));
        }

        [Authorize(ImaarPermissions.NotificationTypes.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _notificationTypeRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.NotificationTypes.Create)]
        public virtual async Task<NotificationTypeDto> CreateAsync(NotificationTypeCreateDto input)
        {

            var notificationType = await _notificationTypeManager.CreateAsync(
            input.Title
            );

            return ObjectMapper.Map<NotificationType, NotificationTypeDto>(notificationType);
        }

        [Authorize(ImaarPermissions.NotificationTypes.Edit)]
        public virtual async Task<NotificationTypeDto> UpdateAsync(Guid id, NotificationTypeUpdateDto input)
        {

            var notificationType = await _notificationTypeManager.UpdateAsync(
            id,
            input.Title, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<NotificationType, NotificationTypeDto>(notificationType);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(NotificationTypeExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _notificationTypeRepository.GetListAsync(input.FilterText, input.Title);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<NotificationType>, List<NotificationTypeExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "NotificationTypes.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.NotificationTypes.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> notificationtypeIds)
        {
            await _notificationTypeRepository.DeleteManyAsync(notificationtypeIds);
        }

        [Authorize(ImaarPermissions.NotificationTypes.Delete)]
        public virtual async Task DeleteAllAsync(GetNotificationTypesInput input)
        {
            await _notificationTypeRepository.DeleteAllAsync(input.FilterText, input.Title);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new NotificationTypeDownloadTokenCacheItem { Token = token },
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