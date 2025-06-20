using Imaar.Medias;
using Imaar.MimeTypes;
using Imaar.MobileResponses;
using Imaar.Vacancies;
using Imaar.Vacancies;
using Imaar.VacancyAdditionalFeatures;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Imaar.Vacancies
{
    public class VacancyManager : VacancyManagerBase
    {
        protected IBlobContainer<MediaContainer> _mediasContainer;
        protected IMediaRepository _mediaRepository;
        protected MediaManager _mediaManager;
        
        public VacancyManager(IVacancyRepository vacancyRepository, IRepository<VacancyAdditionalFeature, Guid> vacancyAdditionalFeatureRepository,
            IBlobContainer<MediaContainer> mediasContainer,
            IMediaRepository mediaRepository,
            MediaManager mediaManager)
            : base(vacancyRepository, vacancyAdditionalFeatureRepository)
        {
            _mediasContainer = mediasContainer;
            _mediaRepository = mediaRepository;
            _mediaManager = mediaManager;
        }
        //</suite-custom-code-autogenerated>

        public virtual async Task<MobileResponse> CreateWithFilesAsync(
            string title,
            string description,
            string location,
            string number,
            DateOnly dateOfPublish,
            BiologicalSex biologicalSex,
            string phoneNumber,
            string? latitude,
            string? longitude,
            string? expectedExperience,
            string? educationLevel,
            string? workSchedule,
            string? employmentType,
            string? languages,
            string? driveLicense,
            string? salary,
            List<Guid> vacancyAdditionalFeatureIds,
            Guid serviceTypeId,
            Guid userProfileId,
            List<IFormFile> files)
        {
            MobileResponse mobileResponse = new MobileResponse();
            
            try
            {
                // Create the Vacancy first
                var vacancy = await CreateAsync(
                    vacancyAdditionalFeatureIds,
                    serviceTypeId,
                    userProfileId,
                    title,
                    description,
                    location,
                    number,
                    dateOfPublish,
                    biologicalSex,
                    phoneNumber,
                    0,
                    0,
                    latitude,
                    longitude,
                    expectedExperience,
                    educationLevel,
                    workSchedule,
                    employmentType,
                    languages,
                    driveLicense,
                    salary
                );
                
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
                        MediaEntityType.Vacancy,
                        vacancy.Id.ToString(),
                        title
                    );
                }
                
                mobileResponse.Code = 200;
                mobileResponse.Message = "Vacancy created successfully";
                mobileResponse.Data = new { VacancyId = vacancy.Id, FileNames = fileNames };
                
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