using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.StoryLovers
{
    public abstract class StoryLoverManagerBase : DomainService
    {
        protected IStoryLoverRepository _storyLoverRepository;

        public StoryLoverManagerBase(IStoryLoverRepository storyLoverRepository)
        {
            _storyLoverRepository = storyLoverRepository;
        }

        public virtual async Task<StoryLover> CreateAsync(
        Guid userProfileId, Guid storyId)
        {
            Check.NotNull(userProfileId, nameof(userProfileId));
            Check.NotNull(storyId, nameof(storyId));

            var storyLover = new StoryLover(
             GuidGenerator.Create(),
             userProfileId, storyId
             );

            return await _storyLoverRepository.InsertAsync(storyLover);
        }

        public virtual async Task<StoryLover> UpdateAsync(
            Guid id,
            Guid userProfileId, Guid storyId, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(userProfileId, nameof(userProfileId));
            Check.NotNull(storyId, nameof(storyId));

            var storyLover = await _storyLoverRepository.GetAsync(id);

            storyLover.UserProfileId = userProfileId;
            storyLover.StoryId = storyId;

            storyLover.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _storyLoverRepository.UpdateAsync(storyLover);
        }

    }
}