using Imaar.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.Notifications;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.Notifications
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Notification")]
    [Route("api/app/notifications")]

    public abstract class NotificationControllerBase : AbpController
    {
        protected INotificationsAppService _notificationsAppService;

        public NotificationControllerBase(INotificationsAppService notificationsAppService)
        {
            _notificationsAppService = notificationsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<NotificationWithNavigationPropertiesDto>> GetListAsync(GetNotificationsInput input)
        {
            return _notificationsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<NotificationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _notificationsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<NotificationDto> GetAsync(Guid id)
        {
            return _notificationsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("user-profile-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input)
        {
            return _notificationsAppService.GetUserProfileLookupAsync(input);
        }

        [HttpGet]
        [Route("notification-type-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetNotificationTypeLookupAsync(LookupRequestDto input)
        {
            return _notificationsAppService.GetNotificationTypeLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<NotificationDto> CreateAsync(NotificationCreateDto input)
        {
            return _notificationsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<NotificationDto> UpdateAsync(Guid id, NotificationUpdateDto input)
        {
            return _notificationsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _notificationsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(NotificationExcelDownloadDto input)
        {
            return _notificationsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _notificationsAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> notificationIds)
        {
            return _notificationsAppService.DeleteByIdsAsync(notificationIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetNotificationsInput input)
        {
            return _notificationsAppService.DeleteAllAsync(input);
        }
    }
}