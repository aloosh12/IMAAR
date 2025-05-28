using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.NotificationTypes
{
    public abstract class NotificationTypeManagerBase : DomainService
    {
        protected INotificationTypeRepository _notificationTypeRepository;

        public NotificationTypeManagerBase(INotificationTypeRepository notificationTypeRepository)
        {
            _notificationTypeRepository = notificationTypeRepository;
        }

        public virtual async Task<NotificationType> CreateAsync(
        string title)
        {
            Check.NotNullOrWhiteSpace(title, nameof(title));

            var notificationType = new NotificationType(
             GuidGenerator.Create(),
             title
             );

            return await _notificationTypeRepository.InsertAsync(notificationType);
        }

        public virtual async Task<NotificationType> UpdateAsync(
            Guid id,
            string title, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(title, nameof(title));

            var notificationType = await _notificationTypeRepository.GetAsync(id);

            notificationType.Title = title;

            notificationType.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _notificationTypeRepository.UpdateAsync(notificationType);
        }

    }
}