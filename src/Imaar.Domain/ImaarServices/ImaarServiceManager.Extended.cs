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
using static System.Net.WebRequestMethods;

namespace Imaar.ImaarServices
{
    public class ImaarServiceManager : ImaarServiceManagerBase
    {
        protected IBlobContainer<MediaContainer> _mediasContainer;
        protected IMediaRepository _mediaRepository;
        protected MediaManager _mediaManager;
        
        public ImaarServiceManager(
            IImaarServiceRepository imaarServiceRepository,
            IBlobContainer<MediaContainer> mediasContainer,
            IMediaRepository mediaRepository,
            MediaManager mediaManager)
            : base(imaarServiceRepository)
        {
            _mediasContainer = mediasContainer;
            _mediaRepository = mediaRepository;
            _mediaManager = mediaManager;
        }

        public virtual async Task<MobileResponse> CreateWithFilesAsync(
            string title,
            string description,
            string serviceLocation,
            string serviceNumber,
            DateOnly dateOfPublish,
            int price,
            string? latitude,
            string? longitude,
            Guid serviceTypeId,
            Guid userProfileId,
            List<IFormFile> files)
        {
            MobileResponse mobileResponse = new MobileResponse();
            
            try
            {
                // Create the ImaarService first
                var imaarService = await CreateAsync(
                    serviceTypeId,
                    userProfileId,
                    title,
                    description,
                    serviceLocation,
                    serviceNumber,
                    dateOfPublish,
                    price,
                    latitude,
                    longitude
                );
                
                // Process each file
                var fileNames = new List<string>();
                int order = 0;
                string defaultMedia = "";
                
                foreach (var file in files)
                {
                    string fileName = await UploadImage(file);
                    // Set the first image as default media
                    if (order == 0)
                    {
                        defaultMedia = fileName;
                    }
                    
                    fileNames.Add($"{MimeTypeMap.GetAttachmentPath()}/MediaImages/{fileName}");

                    // Create media entry for each file
                    await _mediaManager.CreateAsync(
                        fileName,
                        order++,
                        true,
                        MediaEntityType.Service,
                        imaarService.Id.ToString(),
                        title
                    );
                }
                
                // Update the ImaarService with the default media
                if (!string.IsNullOrEmpty(defaultMedia))
                {
                    imaarService.DefaultMedia = defaultMedia;
                    await _imaarServiceRepository.UpdateAsync(imaarService);
                }
                
                mobileResponse.Code = 200;
                mobileResponse.Message = "ImaarService created successfully";
                mobileResponse.Data = new { ImaarServiceId = imaarService.Id, FileNames = fileNames };
                
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
    }
}