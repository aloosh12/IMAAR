using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.Stories
{
    public abstract class StoryManagerBase : DomainService
    {
        protected IStoryRepository _storyRepository;

        public StoryManagerBase(IStoryRepository storyRepository)
        {
            _storyRepository = storyRepository;
        }

        public virtual async Task<Story> CreateAsync(
        Guid? storyPublisherId, DateTime fromTime, DateTime expiryTime, string? title = null)
        {
            Check.NotNull(fromTime, nameof(fromTime));
            Check.NotNull(expiryTime, nameof(expiryTime));

            var story = new Story(
             GuidGenerator.Create(),
             storyPublisherId, fromTime, expiryTime, title
             );

            return await _storyRepository.InsertAsync(story);
        }

        public virtual async Task<Story> UpdateAsync(
            Guid id,
            Guid? storyPublisherId, DateTime fromTime, DateTime expiryTime, string? title = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(fromTime, nameof(fromTime));
            Check.NotNull(expiryTime, nameof(expiryTime));

            var story = await _storyRepository.GetAsync(id);

            story.StoryPublisherId = storyPublisherId;
            story.FromTime = fromTime;
            story.ExpiryTime = expiryTime;
            story.Title = title;

            story.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _storyRepository.UpdateAsync(story);
        }

    }
}