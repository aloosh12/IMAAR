using Imaar.Notifications;
using Imaar.NotificationTypes;
using Imaar.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Imaar.EntityFrameworkCore;

namespace Imaar.Notifications
{
    public abstract class EfCoreNotificationRepositoryBase : EfCoreRepository<ImaarDbContext, Notification, Guid>
    {
        public EfCoreNotificationRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, title, message, isRead, readDateMin, readDateMax, priorityMin, priorityMax, sourceEntityType, sourceEntityId, senderUserId, userProfileId, notificationTypeId);

            var ids = query.Select(x => x.Notification.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<NotificationWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(notification => new NotificationWithNavigationProperties
                {
                    Notification = notification,
                    UserProfile = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == notification.UserProfileId),
                    NotificationType = dbContext.Set<NotificationType>().FirstOrDefault(c => c.Id == notification.NotificationTypeId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<NotificationWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, title, message, isRead, readDateMin, readDateMax, priorityMin, priorityMax, sourceEntityType, sourceEntityId, senderUserId, userProfileId, notificationTypeId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? NotificationConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<NotificationWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from notification in (await GetDbSetAsync())
                   join userProfile in (await GetDbContextAsync()).Set<UserProfile>() on notification.UserProfileId equals userProfile.Id into userProfiles
                   from userProfile in userProfiles.DefaultIfEmpty()
                   join notificationType in (await GetDbContextAsync()).Set<NotificationType>() on notification.NotificationTypeId equals notificationType.Id into notificationTypes
                   from notificationType in notificationTypes.DefaultIfEmpty()
                   select new NotificationWithNavigationProperties
                   {
                       Notification = notification,
                       UserProfile = userProfile,
                       NotificationType = notificationType
                   };
        }

        protected virtual IQueryable<NotificationWithNavigationProperties> ApplyFilter(
            IQueryable<NotificationWithNavigationProperties> query,
            string? filterText,
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
            Guid? notificationTypeId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Notification.Title!.Contains(filterText!) || e.Notification.Message!.Contains(filterText!) || e.Notification.SourceEntityId!.Contains(filterText!) || e.Notification.SenderUserId!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(title), e => e.Notification.Title.Contains(title))
                    .WhereIf(!string.IsNullOrWhiteSpace(message), e => e.Notification.Message.Contains(message))
                    .WhereIf(isRead.HasValue, e => e.Notification.IsRead == isRead)
                    .WhereIf(readDateMin.HasValue, e => e.Notification.ReadDate >= readDateMin!.Value)
                    .WhereIf(readDateMax.HasValue, e => e.Notification.ReadDate <= readDateMax!.Value)
                    .WhereIf(priorityMin.HasValue, e => e.Notification.Priority >= priorityMin!.Value)
                    .WhereIf(priorityMax.HasValue, e => e.Notification.Priority <= priorityMax!.Value)
                    .WhereIf(sourceEntityType.HasValue, e => e.Notification.SourceEntityType == sourceEntityType)
                    .WhereIf(!string.IsNullOrWhiteSpace(sourceEntityId), e => e.Notification.SourceEntityId.Contains(sourceEntityId))
                    .WhereIf(!string.IsNullOrWhiteSpace(senderUserId), e => e.Notification.SenderUserId.Contains(senderUserId))
                    .WhereIf(userProfileId != null && userProfileId != Guid.Empty, e => e.UserProfile != null && e.UserProfile.Id == userProfileId)
                    .WhereIf(notificationTypeId != null && notificationTypeId != Guid.Empty, e => e.NotificationType != null && e.NotificationType.Id == notificationTypeId);
        }

        public virtual async Task<List<Notification>> GetListAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, title, message, isRead, readDateMin, readDateMax, priorityMin, priorityMax, sourceEntityType, sourceEntityId, senderUserId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? NotificationConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, title, message, isRead, readDateMin, readDateMax, priorityMin, priorityMax, sourceEntityType, sourceEntityId, senderUserId, userProfileId, notificationTypeId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Notification> ApplyFilter(
            IQueryable<Notification> query,
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
            string? senderUserId = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Title!.Contains(filterText!) || e.Message!.Contains(filterText!) || e.SourceEntityId!.Contains(filterText!) || e.SenderUserId!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(title), e => e.Title.Contains(title))
                    .WhereIf(!string.IsNullOrWhiteSpace(message), e => e.Message.Contains(message))
                    .WhereIf(isRead.HasValue, e => e.IsRead == isRead)
                    .WhereIf(readDateMin.HasValue, e => e.ReadDate >= readDateMin!.Value)
                    .WhereIf(readDateMax.HasValue, e => e.ReadDate <= readDateMax!.Value)
                    .WhereIf(priorityMin.HasValue, e => e.Priority >= priorityMin!.Value)
                    .WhereIf(priorityMax.HasValue, e => e.Priority <= priorityMax!.Value)
                    .WhereIf(sourceEntityType.HasValue, e => e.SourceEntityType == sourceEntityType)
                    .WhereIf(!string.IsNullOrWhiteSpace(sourceEntityId), e => e.SourceEntityId.Contains(sourceEntityId))
                    .WhereIf(!string.IsNullOrWhiteSpace(senderUserId), e => e.SenderUserId.Contains(senderUserId));
        }
    }
}