using Imaar.UserSavedItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.UserSavedItems
{
    public abstract class UserSavedItemManagerBase : DomainService
    {
        protected IUserSavedItemRepository _userSavedItemRepository;

        public UserSavedItemManagerBase(IUserSavedItemRepository userSavedItemRepository)
        {
            _userSavedItemRepository = userSavedItemRepository;
        }

        public virtual async Task<UserSavedItem> CreateAsync(
        Guid userProfileId, string sourceId, UserSavedItemType savedItemType)
        {
            Check.NotNull(userProfileId, nameof(userProfileId));
            Check.NotNullOrWhiteSpace(sourceId, nameof(sourceId));
            Check.NotNull(savedItemType, nameof(savedItemType));

            var userSavedItem = new UserSavedItem(
             GuidGenerator.Create(),
             userProfileId, sourceId, savedItemType
             );

            return await _userSavedItemRepository.InsertAsync(userSavedItem);
        }

        public virtual async Task<UserSavedItem> UpdateAsync(
            Guid id,
            Guid userProfileId, string sourceId, UserSavedItemType savedItemType, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(userProfileId, nameof(userProfileId));
            Check.NotNullOrWhiteSpace(sourceId, nameof(sourceId));
            Check.NotNull(savedItemType, nameof(savedItemType));

            var userSavedItem = await _userSavedItemRepository.GetAsync(id);

            userSavedItem.UserProfileId = userProfileId;
            userSavedItem.SourceId = sourceId;
            userSavedItem.SavedItemType = savedItemType;

            userSavedItem.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _userSavedItemRepository.UpdateAsync(userSavedItem);
        }

    }
}