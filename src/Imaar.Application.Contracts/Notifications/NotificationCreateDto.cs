using Imaar.Notifications;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.Notifications
{
    public abstract class NotificationCreateDtoBase
    {
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; } = false;
        public DateTime? ReadDate { get; set; }
        public int? Priority { get; set; }
        public SourceEntityType SourceEntityType { get; set; } = ((SourceEntityType[])Enum.GetValues(typeof(SourceEntityType)))[0];
        public string? SourceEntityId { get; set; }
        public string? SenderUserId { get; set; }
        public Guid UserProfileId { get; set; }
        public Guid NotificationTypeId { get; set; }
    }
}