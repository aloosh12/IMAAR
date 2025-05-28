using System;

namespace Imaar.Notifications;

public abstract class NotificationDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}