using Imaar.UserProfiles;
using Imaar.NotificationTypes;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace Imaar.Notifications
{
    public abstract class NotificationWithNavigationPropertiesDtoBase
    {
        public NotificationDto Notification { get; set; } = null!;

        public UserProfileDto UserProfile { get; set; } = null!;
        public NotificationTypeDto NotificationType { get; set; } = null!;

    }
}