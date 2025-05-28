using Imaar.Notifications;
using System;

namespace Imaar.Notifications
{
    public abstract class NotificationExcelDtoBase
    {
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; }
        public DateTime? ReadDate { get; set; }
        public int? Priority { get; set; }
        public SourceEntityType SourceEntityType { get; set; }
        public string? SourceEntityId { get; set; }
        public string? SenderUserId { get; set; }
    }
}