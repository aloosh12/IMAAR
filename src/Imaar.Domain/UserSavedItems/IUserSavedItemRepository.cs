using Imaar.UserSavedItems;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.UserSavedItems
{
    public partial interface IUserSavedItemRepository : IRepository<UserSavedItem, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? sourceId = null,
            UserSavedItemType? savedItemType = null,
            Guid? userProfileId = null,
            CancellationToken cancellationToken = default);
        Task<UserSavedItemWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<UserSavedItemWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? sourceId = null,
            UserSavedItemType? savedItemType = null,
            Guid? userProfileId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<UserSavedItem>> GetListAsync(
                    string? filterText = null,
                    string? sourceId = null,
                    UserSavedItemType? savedItemType = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? sourceId = null,
            UserSavedItemType? savedItemType = null,
            Guid? userProfileId = null,
            CancellationToken cancellationToken = default);
    }
}