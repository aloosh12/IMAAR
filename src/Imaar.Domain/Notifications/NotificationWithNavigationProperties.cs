using Imaar.UserProfiles;
using Imaar.NotificationTypes;

using System;
using System.Collections.Generic;

namespace Imaar.Notifications
{
    public abstract class NotificationWithNavigationPropertiesBase
    {
        public Notification Notification { get; set; } = null!;

        public UserProfile UserProfile { get; set; } = null!;
        public NotificationType NotificationType { get; set; } = null!;
        

        
    }
}