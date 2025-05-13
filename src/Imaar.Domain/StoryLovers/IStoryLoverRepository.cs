using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.StoryLovers
{
    public partial interface IStoryLoverRepository : IRepository<StoryLover, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            Guid? userProfileId = null,
            Guid? storyId = null,
            CancellationToken cancellationToken = default);
        Task<StoryLoverWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<StoryLoverWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            Guid? userProfileId = null,
            Guid? storyId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<StoryLover>> GetListAsync(
                    string? filterText = null,

                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            Guid? userProfileId = null,
            Guid? storyId = null,
            CancellationToken cancellationToken = default);
    }
}