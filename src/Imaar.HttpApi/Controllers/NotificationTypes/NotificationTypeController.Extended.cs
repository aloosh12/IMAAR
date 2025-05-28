using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.NotificationTypes;

namespace Imaar.Controllers.NotificationTypes
{
    [RemoteService]
    [Area("app")]
    [ControllerName("NotificationType")]
    [Route("api/app/notification-types")]

    public class NotificationTypeController : NotificationTypeControllerBase, INotificationTypesAppService
    {
        public NotificationTypeController(INotificationTypesAppService notificationTypesAppService) : base(notificationTypesAppService)
        {
        }
    }
}