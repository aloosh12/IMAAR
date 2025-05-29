using Imaar.Shared;
using Imaar.UserProfiles;
using Imaar.ServiceTypes;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Imaar.Permissions;
using Imaar.Vacancies;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;
using Imaar.MobileResponses;
using Imaar.Medias;

namespace Imaar.Vacancies
{
    public class VacanciesAppService : VacanciesAppServiceBase, IVacanciesAppService
    {
        protected IMediasAppService _mediasAppService;
        
        public VacanciesAppService(IVacancyRepository vacancyRepository, 
            VacancyManager vacancyManager, 
            IDistributedCache<VacancyDownloadTokenCacheItem, string> downloadTokenCache, 
            IRepository<Imaar.ServiceTypes.ServiceType, Guid> serviceTypeRepository, 
            IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository,
            IMediasAppService mediasAppService)
            : base(vacancyRepository, vacancyManager, downloadTokenCache, serviceTypeRepository, userProfileRepository)
        {
            _mediasAppService = mediasAppService;
        }

        [AllowAnonymous]
        public virtual async Task<MobileResponseDto> CreateWithFilesAsync(VacancyCreateWithFilesDto input)
        {
            var result = await _vacancyManager.CreateWithFilesAsync(
                input.Title,
                input.Description,
                input.Location,
                input.Number,
                input.DateOfPublish,
                input.BiologicalSex,
                input.Latitude,
                input.Longitude,
                input.ExpectedExperience,
                input.EducationLevel,
                input.WorkSchedule,
                input.EmploymentType,
                input.Languages,
                input.DriveLicense,
                input.Salary,
                input.ServiceTypeId,
                input.UserProfileId,
                input.Files
            );
            
            return ObjectMapper.Map<MobileResponse, MobileResponseDto>(result);
        }
    }
}