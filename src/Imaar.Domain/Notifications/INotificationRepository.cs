using Imaar.Notifications;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.Notifications
{
    public partial interface INotificationRepository : IRepository<Notification, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? title = null,
            string? message = null,
            bool? isRead = null,
            DateTime? readDateMin = null,
            DateTime? readDateMax = null,
            int? priorityMin = null,
            int? priorityMax = null,
            SourceEntityType? sourceEntityType = null,
            string? sourceEntityId = null,
            string? senderUserId = null,
            Guid? userProfileId = null,
            Guid? notificationTypeId = null,
            CancellationToken cancellationToken = default);
        Task<NotificationWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<NotificationWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? title = null,
            string? message = null,
            bool? isRead = null,
            DateTime? readDateMin = null,
            DateTime? readDateMax = null,
            int? priorityMin = null,
            int? priorityMax = null,
            SourceEntityType? sourceEntityType = null,
            string? sourceEntityId = null,
            string? senderUserId = null,
            Guid? userProfileId = null,
            Guid? notificationTypeId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<Notification>> GetListAsync(
                    string? filterText = null,
                    string? title = null,
                    string? message = null,
                    bool? isRead = null,
                    DateTime? readDateMin = null,
                    DateTime? readDateMax = null,
                    int? priorityMin = null,
                    int? priorityMax = null,
                    SourceEntityType? sourceEntityType = null,
                    string? sourceEntityId = null,
                    string? senderUserId = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? title = null,
            string? message = null,
            bool? isRead = null,
            DateTime? readDateMin = null,
            DateTime? readDateMax = null,
            int? priorityMin = null,
            int? priorityMax = null,
            SourceEntityType? sourceEntityType = null,
            string? sourceEntityId = null,
            string? senderUserId = null,
            Guid? userProfileId = null,
            Guid? notificationTypeId = null,
            CancellationToken cancellationToken = default);
    }
}