using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.NotificationTypes
{
    public partial interface INotificationTypeRepository : IRepository<NotificationType, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? title = null,
            CancellationToken cancellationToken = default);
        Task<List<NotificationType>> GetListAsync(
                    string? filterText = null,
                    string? title = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? title = null,
            CancellationToken cancellationToken = default);
    }
}