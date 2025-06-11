using Imaar.ImaarServices;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Imaar.ImaarServices
{
    public abstract class ImaarServiceManagerBase : DomainService
    {
        protected IImaarServiceRepository _imaarServiceRepository;

        public ImaarServiceManagerBase(IImaarServiceRepository imaarServiceRepository)
        {
            _imaarServiceRepository = imaarServiceRepository;
        }

        public virtual async Task<ImaarService> CreateAsync(
        Guid serviceTypeId, Guid userProfileId, string title, string description, string serviceLocation, string serviceNumber, DateOnly dateOfPublish, int price, string phoneNumber, int viewCounter, int orderCounter, string? latitude = null, string? longitude = null)
        {
            Check.NotNull(serviceTypeId, nameof(serviceTypeId));
            Check.NotNull(userProfileId, nameof(userProfileId));
            Check.NotNullOrWhiteSpace(title, nameof(title));
            Check.NotNullOrWhiteSpace(description, nameof(description));
            Check.NotNullOrWhiteSpace(serviceLocation, nameof(serviceLocation));
            Check.NotNullOrWhiteSpace(serviceNumber, nameof(serviceNumber));
            Check.NotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));

            var imaarService = new ImaarService(
             GuidGenerator.Create(),
             serviceTypeId, userProfileId, title, description, serviceLocation, serviceNumber, dateOfPublish, price, phoneNumber, viewCounter, orderCounter, latitude, longitude
             );

            return await _imaarServiceRepository.InsertAsync(imaarService);
        }

        public virtual async Task<ImaarService> UpdateAsync(
            Guid id,
            Guid serviceTypeId, Guid userProfileId, string title, string description, string serviceLocation, string serviceNumber, DateOnly dateOfPublish, int price, string phoneNumber, int viewCounter, int orderCounter, string? latitude = null, string? longitude = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(serviceTypeId, nameof(serviceTypeId));
            Check.NotNull(userProfileId, nameof(userProfileId));
            Check.NotNullOrWhiteSpace(title, nameof(title));
            Check.NotNullOrWhiteSpace(description, nameof(description));
            Check.NotNullOrWhiteSpace(serviceLocation, nameof(serviceLocation));
            Check.NotNullOrWhiteSpace(serviceNumber, nameof(serviceNumber));
            Check.NotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));

            var imaarService = await _imaarServiceRepository.GetAsync(id);

            imaarService.ServiceTypeId = serviceTypeId;
            imaarService.UserProfileId = userProfileId;
            imaarService.Title = title;
            imaarService.Description = description;
            imaarService.ServiceLocation = serviceLocation;
            imaarService.ServiceNumber = serviceNumber;
            imaarService.DateOfPublish = dateOfPublish;
            imaarService.Price = price;
            imaarService.PhoneNumber = phoneNumber;
            imaarService.ViewCounter = viewCounter;
            imaarService.OrderCounter = orderCounter;
            imaarService.Latitude = latitude;
            imaarService.Longitude = longitude;

            imaarService.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _imaarServiceRepository.UpdateAsync(imaarService);
        }

    }
}