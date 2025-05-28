using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.NotificationTypes;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.NotificationTypes
{
    [RemoteService]
    [Area("app")]
    [ControllerName("NotificationType")]
    [Route("api/app/notification-types")]

    public abstract class NotificationTypeControllerBase : AbpController
    {
        protected INotificationTypesAppService _notificationTypesAppService;

        public NotificationTypeControllerBase(INotificationTypesAppService notificationTypesAppService)
        {
            _notificationTypesAppService = notificationTypesAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<NotificationTypeDto>> GetListAsync(GetNotificationTypesInput input)
        {
            return _notificationTypesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<NotificationTypeDto> GetAsync(Guid id)
        {
            return _notificationTypesAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<NotificationTypeDto> CreateAsync(NotificationTypeCreateDto input)
        {
            return _notificationTypesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<NotificationTypeDto> UpdateAsync(Guid id, NotificationTypeUpdateDto input)
        {
            return _notificationTypesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _notificationTypesAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(NotificationTypeExcelDownloadDto input)
        {
            return _notificationTypesAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _notificationTypesAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> notificationtypeIds)
        {
            return _notificationTypesAppService.DeleteByIdsAsync(notificationtypeIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetNotificationTypesInput input)
        {
            return _notificationTypesAppService.DeleteAllAsync(input);
        }
    }
}