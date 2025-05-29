using Imaar.UserProfiles;
using System;
using Volo.Abp.Domain.Services;
using Volo.Abp.Domain.Repositories;
using System.Threading.Tasks;
using Volo.Abp;
using Microsoft.AspNetCore.Http;
using System.IO;
using Volo.Abp.BlobStoring;
using System.Collections.Generic;
using Imaar.MobileResponses;
using Imaar.Medias;
using Imaar.MimeTypes;

namespace Imaar.Stories
{
    public class StoryManager : StoryManagerBase
    {
        protected IBlobContainer<MediaContainer> _mediasContainer;
        protected IMediaRepository _mediaRepository;
        protected MediaManager _mediaManager;

        public StoryManager(IStoryRepository storyRepository, IBlobContainer<MediaContainer> mediasContainer, 
            IMediaRepository mediaRepository, MediaManager mediaManager)
            : base(storyRepository)
        {
            _mediasContainer = mediasContainer;
            _mediaRepository = mediaRepository;
            _mediaManager = mediaManager;
        }

        public virtual async Task<MobileResponse> CreateWithFilesAsync(string title, Guid publisherId, List<IFormFile> files)
        {
            MobileResponse mobileResponse = new MobileResponse();
            
            try
            {
                // Create the story first
                var story = await CreateAsync(publisherId, DateTime.Now, DateTime.Now.AddDays(1), title);
                
                // Process each file
                var fileNames = new List<string>();
                int order = 0;
                
                foreach (var file in files)
                {
                    string fileName = await UploadImage(file);
                    fileNames.Add($"{MimeTypeMap.GetAttachmentPath()}/MediaImages/{fileName}");

                    // Create media entry for each file
                    await _mediaManager.CreateAsync(
                        fileName,
                        order++,
                        true,
                        MediaEntityType.Story,
                        story.Id.ToString(),
                        file.FileName
                    );
                }
                
                mobileResponse.Code = 200;
                mobileResponse.Message = "Story created successfully";
                mobileResponse.Data = new { StoryId = story.Id, FileNames = fileNames };
                
                return mobileResponse;
            }
            catch (Exception ex)
            {
                mobileResponse.Code = 500;
                mobileResponse.Message = ex.Message;
                mobileResponse.Data = null;
                return mobileResponse;
            }
        }

        private async Task<string> UploadImage(IFormFile formFile)
        {
            using (var stream = new MemoryStream())
            {
                formFile.CopyTo(stream);
                string imageName = $"{Guid.NewGuid().ToString("N")}{Path.GetExtension(formFile.FileName)}";
                await _mediasContainer.SaveAsync(imageName, stream.GetAllBytes());
                return imageName;
            }
        }

        //Write your custom code...
    }
}