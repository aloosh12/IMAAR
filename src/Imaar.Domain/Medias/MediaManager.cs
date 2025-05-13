using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.Medias
{
    public abstract class MediaManagerBase : DomainService
    {
        protected IMediaRepository _mediaRepository;

        public MediaManagerBase(IMediaRepository mediaRepository)
        {
            _mediaRepository = mediaRepository;
        }

        public virtual async Task<Media> CreateAsync(
        Guid? imaarServiceId, Guid? vacancyId, Guid? storyId, string file, int order, bool isActive, string? title = null)
        {
            Check.NotNullOrWhiteSpace(file, nameof(file));

            var media = new Media(
             GuidGenerator.Create(),
             imaarServiceId, vacancyId, storyId, file, order, isActive, title
             );

            return await _mediaRepository.InsertAsync(media);
        }

        public virtual async Task<Media> UpdateAsync(
            Guid id,
            Guid? imaarServiceId, Guid? vacancyId, Guid? storyId, string file, int order, bool isActive, string? title = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(file, nameof(file));

            var media = await _mediaRepository.GetAsync(id);

            media.ImaarServiceId = imaarServiceId;
            media.VacancyId = vacancyId;
            media.StoryId = storyId;
            media.File = file;
            media.Order = order;
            media.IsActive = isActive;
            media.Title = title;

            media.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _mediaRepository.UpdateAsync(media);
        }

    }
}