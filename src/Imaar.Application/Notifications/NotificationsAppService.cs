using Imaar.Shared;
using Imaar.NotificationTypes;
using Imaar.UserProfiles;
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
using Imaar.Notifications;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.Notifications
{

    [Authorize(ImaarPermissions.Notifications.Default)]
    public abstract class NotificationsAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<NotificationDownloadTokenCacheItem, string> _downloadTokenCache;
        protected INotificationRepository _notificationRepository;
        protected NotificationManager _notificationManager;

        protected IRepository<Imaar.UserProfiles.UserProfile, Guid> _userProfileRepository;
        protected IRepository<Imaar.NotificationTypes.NotificationType, Guid> _notificationTypeRepository;

        public NotificationsAppServiceBase(INotificationRepository notificationRepository, NotificationManager notificationManager, IDistributedCache<NotificationDownloadTokenCacheItem, string> downloadTokenCache, IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository, IRepository<Imaar.NotificationTypes.NotificationType, Guid> notificationTypeRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _notificationRepository = notificationRepository;
            _notificationManager = notificationManager; _userProfileRepository = userProfileRepository;
            _notificationTypeRepository = notificationTypeRepository;

        }

        public virtual async Task<PagedResultDto<NotificationWithNavigationPropertiesDto>> GetListAsync(GetNotificationsInput input)
        {
            var totalCount = await _notificationRepository.GetCountAsync(input.FilterText, input.Title, input.Message, input.IsRead, input.ReadDateMin, input.ReadDateMax, input.PriorityMin, input.PriorityMax, input.SourceEntityType, input.SourceEntityId, input.SenderUserId, input.UserProfileId, input.NotificationTypeId);
            var items = await _notificationRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Title, input.Message, input.IsRead, input.ReadDateMin, input.ReadDateMax, input.PriorityMin, input.PriorityMax, input.SourceEntityType, input.SourceEntityId, input.SenderUserId, input.UserProfileId, input.NotificationTypeId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<NotificationWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<NotificationWithNavigationProperties>, List<NotificationWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<NotificationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<NotificationWithNavigationProperties, NotificationWithNavigationPropertiesDto>
                (await _notificationRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<NotificationDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Notification, NotificationDto>(await _notificationRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input)
        {
            var query = (await _userProfileRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.SecurityNumber != null &&
                         x.SecurityNumber.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.UserProfiles.UserProfile>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.UserProfiles.UserProfile>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetNotificationTypeLookupAsync(LookupRequestDto input)
        {
            var query = (await _notificationTypeRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Title != null &&
                         x.Title.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.NotificationTypes.NotificationType>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.NotificationTypes.NotificationType>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(ImaarPermissions.Notifications.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _notificationRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.Notifications.Create)]
        public virtual async Task<NotificationDto> CreateAsync(NotificationCreateDto input)
        {
            if (input.UserProfileId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.NotificationTypeId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["NotificationType"]]);
            }

            var notification = await _notificationManager.CreateAsync(
            input.UserProfileId, input.NotificationTypeId, input.Title, input.Message, input.IsRead, input.SourceEntityType, input.ReadDate, input.Priority, input.SourceEntityId, input.SenderUserId
            );

            return ObjectMapper.Map<Notification, NotificationDto>(notification);
        }

        [Authorize(ImaarPermissions.Notifications.Edit)]
        public virtual async Task<NotificationDto> UpdateAsync(Guid id, NotificationUpdateDto input)
        {
            if (input.UserProfileId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.NotificationTypeId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["NotificationType"]]);
            }

            var notification = await _notificationManager.UpdateAsync(
            id,
            input.UserProfileId, input.NotificationTypeId, input.Title, input.Message, input.IsRead, input.SourceEntityType, input.ReadDate, input.Priority, input.SourceEntityId, input.SenderUserId, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Notification, NotificationDto>(notification);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(NotificationExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var notifications = await _notificationRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Title, input.Message, input.IsRead, input.ReadDateMin, input.ReadDateMax, input.PriorityMin, input.PriorityMax, input.SourceEntityType, input.SourceEntityId, input.SenderUserId, input.UserProfileId, input.NotificationTypeId);
            var items = notifications.Select(item => new
            {
                Title = item.Notification.Title,
                Message = item.Notification.Message,
                IsRead = item.Notification.IsRead,
                ReadDate = item.Notification.ReadDate,
                Priority = item.Notification.Priority,
                SourceEntityType = item.Notification.SourceEntityType,
                SourceEntityId = item.Notification.SourceEntityId,
                SenderUserId = item.Notification.SenderUserId,

                UserProfile = item.UserProfile?.SecurityNumber,
                NotificationType = item.NotificationType?.Title,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Notifications.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.Notifications.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> notificationIds)
        {
            await _notificationRepository.DeleteManyAsync(notificationIds);
        }

        [Authorize(ImaarPermissions.Notifications.Delete)]
        public virtual async Task DeleteAllAsync(GetNotificationsInput input)
        {
            await _notificationRepository.DeleteAllAsync(input.FilterText, input.Title, input.Message, input.IsRead, input.ReadDateMin, input.ReadDateMax, input.PriorityMin, input.PriorityMax, input.SourceEntityType, input.SourceEntityId, input.SenderUserId, input.UserProfileId, input.NotificationTypeId);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new NotificationDownloadTokenCacheItem { Token = token },
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