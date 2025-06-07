using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Imaar.Stories
{
    public partial interface IStoryRepository
    {
        Task<List<StoryWithNavigationProperties>> GetListWithNavigationPropertiesByStoryIdsAsync(
            List<Guid> storyIds,
            CancellationToken cancellationToken = default
        );
    }
}