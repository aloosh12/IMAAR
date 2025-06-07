using Imaar.EntityFrameworkCore;
using Imaar.Medias;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Imaar.Stories
{
    public class EfCoreStoryRepository : EfCoreStoryRepositoryBase, IStoryRepository
    {
        public EfCoreStoryRepository(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<StoryWithNavigationProperties>> GetListWithNavigationPropertiesByStoryIdsAsync(
            List<Guid> storyIds, 
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            
            // Filter by story IDs
            query = query.Where(swp => storyIds.Contains(swp.Story.Id));
            
            // Include media related to the stories
            var dbContext = await GetDbContextAsync();
            var result = await query.ToListAsync(cancellationToken);
            
            // For each story, load its related media
            foreach (var item in result)
            {
                item.Medias = await dbContext.Set<Imaar.Medias.Media>()
                    .Where(m => m.SourceEntityType == MediaEntityType.Story && m.SourceEntityId == item.Story.Id.ToString())
                    .ToListAsync(cancellationToken);
            }
            
            return result;
        }
    }
}