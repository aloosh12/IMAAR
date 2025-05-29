using Imaar.Medias;
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
        string file, int order, bool isActive, MediaEntityType sourceEntityType, string sourceEntityId, string? title = null)
        {
            Check.NotNullOrWhiteSpace(file, nameof(file));
            Check.NotNull(sourceEntityType, nameof(sourceEntityType));
            Check.NotNullOrWhiteSpace(sourceEntityId, nameof(sourceEntityId));

            var media = new Media(
             GuidGenerator.Create(),
             file, order, isActive, sourceEntityType, sourceEntityId, title
             );

            return await _mediaRepository.InsertAsync(media);
        }

        public virtual async Task<Media> UpdateAsync(
            Guid id,
            string file, int order, bool isActive, MediaEntityType sourceEntityType, string sourceEntityId, string? title = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(file, nameof(file));
            Check.NotNull(sourceEntityType, nameof(sourceEntityType));
            Check.NotNullOrWhiteSpace(sourceEntityId, nameof(sourceEntityId));

            var media = await _mediaRepository.GetAsync(id);

            media.File = file;
            media.Order = order;
            media.IsActive = isActive;
            media.SourceEntityType = sourceEntityType;
            media.SourceEntityId = sourceEntityId;
            media.Title = title;

            media.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _mediaRepository.UpdateAsync(media);
        }

    }
}