using Imaar.Notifications;
using Imaar.UserProfiles;
using Imaar.NotificationTypes;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace Imaar.Notifications
{
    public abstract class NotificationBase : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Title { get; set; }

        [NotNull]
        public virtual string Message { get; set; }

        public virtual bool IsRead { get; set; }

        public virtual DateTime? ReadDate { get; set; }

        public virtual int? Priority { get; set; }

        public virtual SourceEntityType SourceEntityType { get; set; }

        [CanBeNull]
        public virtual string? SourceEntityId { get; set; }

        [CanBeNull]
        public virtual string? SenderUserId { get; set; }
        public Guid UserProfileId { get; set; }
        public Guid NotificationTypeId { get; set; }

        protected NotificationBase()
        {

        }

        public NotificationBase(Guid id, Guid userProfileId, Guid notificationTypeId, string title, string message, bool isRead, SourceEntityType sourceEntityType, DateTime? readDate = null, int? priority = null, string? sourceEntityId = null, string? senderUserId = null)
        {

            Id = id;
            Check.NotNull(title, nameof(title));
            Check.NotNull(message, nameof(message));
            Title = title;
            Message = message;
            IsRead = isRead;
            SourceEntityType = sourceEntityType;
            ReadDate = readDate;
            Priority = priority;
            SourceEntityId = sourceEntityId;
            SenderUserId = senderUserId;
            UserProfileId = userProfileId;
            NotificationTypeId = notificationTypeId;
        }

    }
}