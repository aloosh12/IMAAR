using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.Stories
{
    public partial interface IStoryRepository : IRepository<Story, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? title = null,
            DateTime? fromTimeMin = null,
            DateTime? fromTimeMax = null,
            DateTime? expiryTimeMin = null,
            DateTime? expiryTimeMax = null,
            Guid? storyPublisherId = null,
            CancellationToken cancellationToken = default);
        Task<StoryWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<StoryWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? title = null,
            DateTime? fromTimeMin = null,
            DateTime? fromTimeMax = null,
            DateTime? expiryTimeMin = null,
            DateTime? expiryTimeMax = null,
            Guid? storyPublisherId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<Story>> GetListAsync(
                    string? filterText = null,
                    string? title = null,
                    DateTime? fromTimeMin = null,
                    DateTime? fromTimeMax = null,
                    DateTime? expiryTimeMin = null,
                    DateTime? expiryTimeMax = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? title = null,
            DateTime? fromTimeMin = null,
            DateTime? fromTimeMax = null,
            DateTime? expiryTimeMin = null,
            DateTime? expiryTimeMax = null,
            Guid? storyPublisherId = null,
            CancellationToken cancellationToken = default);
    }
}