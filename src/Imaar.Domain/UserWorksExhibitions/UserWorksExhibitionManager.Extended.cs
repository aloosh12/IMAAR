using System;
using Volo.Abp.Domain.Services;
using Volo.Abp.Domain.Repositories;
using System.Threading.Tasks;
using Volo.Abp;
using Microsoft.AspNetCore.Http;
using Imaar.UserProfiles;
using System.IO;
using Volo.Abp.BlobStoring;

namespace Imaar.UserWorksExhibitions
{
    public class UserWorksExhibitionManager : UserWorksExhibitionManagerBase
    {
        protected IBlobContainer<UserWorksExhibitionsContainer> _userWorksExhibitionsContainer;
        //<suite-custom-code-autogenerated>
        public UserWorksExhibitionManager(IUserWorksExhibitionRepository userWorksExhibitionRepository, IBlobContainer<UserWorksExhibitionsContainer> userWorksExhibitionsContainer)
            : base(userWorksExhibitionRepository)
        {
            _userWorksExhibitionsContainer = userWorksExhibitionsContainer;
        }
        //</suite-custom-code-autogenerated>
        public virtual async Task<UserWorksExhibition> CreateMobileAsync(
Guid userProfileId, IFormFile file, int order, string? title = null)
        {
            Check.NotNull(userProfileId, nameof(userProfileId));
            //Check.NotNullOrWhiteSpace(file, nameof(file));
            var imageName = await uploadImage(file);
            var userWorksExhibition = new UserWorksExhibition(
             GuidGenerator.Create(),
             userProfileId, imageName, order, title
             );

            return await _userWorksExhibitionRepository.InsertAsync(userWorksExhibition);
        }

        private async Task<string> uploadImage(IFormFile formFile)
        {
            using (var stream = new MemoryStream())
            {
                formFile.CopyTo(stream);
                string imageName = $"{Guid.NewGuid().ToString("N")}{Path.GetExtension(formFile.FileName)}";
                await _userWorksExhibitionsContainer.SaveAsync(imageName, stream.GetAllBytes());
                return imageName;
            }
        }
    }
}