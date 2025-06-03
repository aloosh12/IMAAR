using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.StoryTicketTypes
{
    public abstract class StoryTicketTypeManagerBase : DomainService
    {
        protected IStoryTicketTypeRepository _storyTicketTypeRepository;

        public StoryTicketTypeManagerBase(IStoryTicketTypeRepository storyTicketTypeRepository)
        {
            _storyTicketTypeRepository = storyTicketTypeRepository;
        }

        public virtual async Task<StoryTicketType> CreateAsync(
        string title, int order, bool isActive)
        {
            Check.NotNullOrWhiteSpace(title, nameof(title));

            var storyTicketType = new StoryTicketType(
             GuidGenerator.Create(),
             title, order, isActive
             );

            return await _storyTicketTypeRepository.InsertAsync(storyTicketType);
        }

        public virtual async Task<StoryTicketType> UpdateAsync(
            Guid id,
            string title, int order, bool isActive, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(title, nameof(title));

            var storyTicketType = await _storyTicketTypeRepository.GetAsync(id);

            storyTicketType.Title = title;
            storyTicketType.Order = order;
            storyTicketType.IsActive = isActive;

            storyTicketType.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _storyTicketTypeRepository.UpdateAsync(storyTicketType);
        }

    }
}