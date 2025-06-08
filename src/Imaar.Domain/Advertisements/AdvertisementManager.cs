using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.Advertisements
{
    public abstract class AdvertisementManagerBase : DomainService
    {
        protected IAdvertisementRepository _advertisementRepository;

        public AdvertisementManagerBase(IAdvertisementRepository advertisementRepository)
        {
            _advertisementRepository = advertisementRepository;
        }

        public virtual async Task<Advertisement> CreateAsync(
        Guid? userProfileId, string file, int order, bool isActive, string? title = null, string? subTitle = null, DateTime? fromDateTime = null, DateTime? toDateTime = null)
        {
            Check.NotNullOrWhiteSpace(file, nameof(file));

            var advertisement = new Advertisement(
             GuidGenerator.Create(),
             userProfileId, file, order, isActive, title, subTitle, fromDateTime, toDateTime
             );

            return await _advertisementRepository.InsertAsync(advertisement);
        }

        public virtual async Task<Advertisement> UpdateAsync(
            Guid id,
            Guid? userProfileId, string file, int order, bool isActive, string? title = null, string? subTitle = null, DateTime? fromDateTime = null, DateTime? toDateTime = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(file, nameof(file));

            var advertisement = await _advertisementRepository.GetAsync(id);

            advertisement.UserProfileId = userProfileId;
            advertisement.File = file;
            advertisement.Order = order;
            advertisement.IsActive = isActive;
            advertisement.Title = title;
            advertisement.SubTitle = subTitle;
            advertisement.FromDateTime = fromDateTime;
            advertisement.ToDateTime = toDateTime;

            advertisement.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _advertisementRepository.UpdateAsync(advertisement);
        }

    }
}