using Imaar.Notifications;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Imaar.Notifications
{
    public abstract class NotificationUpdateDtoBase : IHasConcurrencyStamp
    {
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; }
        public DateTime? ReadDate { get; set; }
        public int? Priority { get; set; }
        public SourceEntityType SourceEntityType { get; set; }
        public string? SourceEntityId { get; set; }
        public string? SenderUserId { get; set; }
        public Guid UserProfileId { get; set; }
        public Guid NotificationTypeId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}