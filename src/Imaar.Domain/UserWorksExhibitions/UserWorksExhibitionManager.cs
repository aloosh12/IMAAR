using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.UserWorksExhibitions
{
    public abstract class UserWorksExhibitionManagerBase : DomainService
    {
        protected IUserWorksExhibitionRepository _userWorksExhibitionRepository;

        public UserWorksExhibitionManagerBase(IUserWorksExhibitionRepository userWorksExhibitionRepository)
        {
            _userWorksExhibitionRepository = userWorksExhibitionRepository;
        }

        public virtual async Task<UserWorksExhibition> CreateAsync(
        Guid userProfileId, string file, int order, string? title = null)
        {
            Check.NotNull(userProfileId, nameof(userProfileId));
            Check.NotNullOrWhiteSpace(file, nameof(file));

            var userWorksExhibition = new UserWorksExhibition(
             GuidGenerator.Create(),
             userProfileId, file, order, title
             );

            return await _userWorksExhibitionRepository.InsertAsync(userWorksExhibition);
        }

        public virtual async Task<UserWorksExhibition> UpdateAsync(
            Guid id,
            Guid userProfileId, string file, int order, string? title = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(userProfileId, nameof(userProfileId));
            Check.NotNullOrWhiteSpace(file, nameof(file));

            var userWorksExhibition = await _userWorksExhibitionRepository.GetAsync(id);

            userWorksExhibition.UserProfileId = userProfileId;
            userWorksExhibition.File = file;
            userWorksExhibition.Order = order;
            userWorksExhibition.Title = title;

            userWorksExhibition.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _userWorksExhibitionRepository.UpdateAsync(userWorksExhibition);
        }

    }
}