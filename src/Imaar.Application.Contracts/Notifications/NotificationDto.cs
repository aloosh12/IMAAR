using Imaar.Notifications;
using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Imaar.Notifications
{
    public abstract class NotificationDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Title { get; set; } = null!;
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