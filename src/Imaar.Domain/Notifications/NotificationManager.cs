using Imaar.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.Notifications
{
    public abstract class NotificationManagerBase : DomainService
    {
        protected INotificationRepository _notificationRepository;

        public NotificationManagerBase(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public virtual async Task<Notification> CreateAsync(
        Guid userProfileId, Guid notificationTypeId, string title, string message, bool isRead, SourceEntityType sourceEntityType, DateTime? readDate = null, int? priority = null, string? sourceEntityId = null, string? senderUserId = null)
        {
            Check.NotNull(userProfileId, nameof(userProfileId));
            Check.NotNull(notificationTypeId, nameof(notificationTypeId));
            Check.NotNullOrWhiteSpace(title, nameof(title));
            Check.NotNullOrWhiteSpace(message, nameof(message));
            Check.NotNull(sourceEntityType, nameof(sourceEntityType));

            var notification = new Notification(
             GuidGenerator.Create(),
             userProfileId, notificationTypeId, title, message, isRead, sourceEntityType, readDate, priority, sourceEntityId, senderUserId
             );

            return await _notificationRepository.InsertAsync(notification);
        }

        public virtual async Task<Notification> UpdateAsync(
            Guid id,
            Guid userProfileId, Guid notificationTypeId, string title, string message, bool isRead, SourceEntityType sourceEntityType, DateTime? readDate = null, int? priority = null, string? sourceEntityId = null, string? senderUserId = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(userProfileId, nameof(userProfileId));
            Check.NotNull(notificationTypeId, nameof(notificationTypeId));
            Check.NotNullOrWhiteSpace(title, nameof(title));
            Check.NotNullOrWhiteSpace(message, nameof(message));
            Check.NotNull(sourceEntityType, nameof(sourceEntityType));

            var notification = await _notificationRepository.GetAsync(id);

            notification.UserProfileId = userProfileId;
            notification.NotificationTypeId = notificationTypeId;
            notification.Title = title;
            notification.Message = message;
            notification.IsRead = isRead;
            notification.SourceEntityType = sourceEntityType;
            notification.ReadDate = readDate;
            notification.Priority = priority;
            notification.SourceEntityId = sourceEntityId;
            notification.SenderUserId = senderUserId;

            notification.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _notificationRepository.UpdateAsync(notification);
        }

    }
}