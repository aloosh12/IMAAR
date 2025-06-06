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
        
        [AllowAnonymous]
        public virtual async Task<MobileResponseDto> GetVacancyWithDetailsAsync(Guid id)
        {
            var mobileResponse = new MobileResponse();
            
            try
            {
                // Get the Vacancy with navigation properties
                var vacancyWithDetails = await _vacancyRepository.GetWithNavigationPropertiesAsync(id);
                
                if (vacancyWithDetails == null || vacancyWithDetails.Vacancy == null)
                {
                    mobileResponse.Code = 404;
                    mobileResponse.Message = "Vacancy not found";
                    mobileResponse.Data = null;
                    
                    return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
                }
                
                // Map to DTO
                var vacancyDto = ObjectMapper.Map<Vacancy, VacancyDto>(vacancyWithDetails.Vacancy);
                var serviceTypeDto = ObjectMapper.Map<ServiceType, ServiceTypeDto>(vacancyWithDetails.ServiceType);
                var userProfileDto = ObjectMapper.Map<UserProfile, UserProfileDto>(vacancyWithDetails.UserProfile);

                // Get media for this vacancy
                GetMediasInput getMediasInput = new GetMediasInput();
                getMediasInput.SkipCount = 0;
                getMediasInput.MaxResultCount = 1000;
                getMediasInput.SourceEntityId = id.ToString();
                getMediasInput.SourceEntityType = MediaEntityType.Vacancy;
                getMediasInput.IsActive = true;
                getMediasInput.Sorting = "Order asc";
                var mediaListDto = await _mediasAppService.GetListAsync(getMediasInput);
                
                // Create result DTO
                var result = new VacancyWithDetailsMobileDto
                {
                    // Copy properties from vacancyDto
                    Id = vacancyDto.Id,
                    Title = vacancyDto.Title,
                    Description = vacancyDto.Description,
                    Location = vacancyDto.Location,
                    Number = vacancyDto.Number,
                    Latitude = vacancyDto.Latitude,
                    Longitude = vacancyDto.Longitude,
                    DateOfPublish = vacancyDto.DateOfPublish,
                    ExpectedExperience = vacancyDto.ExpectedExperience,
                    EducationLevel = vacancyDto.EducationLevel,
                    WorkSchedule = vacancyDto.WorkSchedule,
                    EmploymentType = vacancyDto.EmploymentType,
                    BiologicalSex = vacancyDto.BiologicalSex,
                    Languages = vacancyDto.Languages,
                    DriveLicense = vacancyDto.DriveLicense,
                    Salary = vacancyDto.Salary,
                    ViewCounter = vacancyDto.ViewCounter,
                    OrderCounter = vacancyDto.OrderCounter,
                    ServiceTypeId = vacancyDto.ServiceTypeId,
                    UserProfileId = vacancyDto.UserProfileId,
                    ConcurrencyStamp = vacancyDto.ConcurrencyStamp,
                    CreationTime = vacancyDto.CreationTime,
                    CreatorId = vacancyDto.CreatorId,
                    LastModificationTime = vacancyDto.LastModificationTime,
                    LastModifierId = vacancyDto.LastModifierId,
                    IsDeleted = vacancyDto.IsDeleted,
                    DeleterId = vacancyDto.DeleterId,
                    DeletionTime = vacancyDto.DeletionTime,
                    
                    // Add navigation properties
                    ServiceType = serviceTypeDto,
                    UserProfile = userProfileDto,
                    Media = mediaListDto != null ?mediaListDto.Items.ToList() : new List<MediaDto>(),
                };
                
                // Increment the view counter asynchronously (fire and forget)
                _ = IncrementViewCounterAsync(id);
                
                mobileResponse.Code = 200;
                mobileResponse.Message = "Vacancy retrieved successfully";
                mobileResponse.Data = result;
                
                return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
            }
            catch (Exception ex)
            {
                mobileResponse.Code = 500;
                mobileResponse.Message = ex.Message;
                mobileResponse.Data = null;
                
                return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
            }
        }
        
        [AllowAnonymous]
        public virtual async Task<MobileResponseDto> IncrementViewCounterAsync(Guid id)
        {
            var mobileResponse = new MobileResponse();
            
            try
            {
                // Get the Vacancy
                var vacancy = await _vacancyRepository.GetAsync(id);
                
                if (vacancy == null)
                {
                    mobileResponse.Code = 404;
                    mobileResponse.Message = "Vacancy not found";
                    mobileResponse.Data = null;
                    
                    return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
                }
                
                // Increment the view counter
                vacancy.ViewCounter += 1;
                
                // Update the entity
                await _vacancyRepository.UpdateAsync(vacancy);
                
                mobileResponse.Code = 200;
                mobileResponse.Message = "View counter incremented successfully";
                mobileResponse.Data = new { 
                    VacancyId = vacancy.Id, 
                    ViewCounter = vacancy.ViewCounter 
                };
                
                return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
            }
            catch (Exception ex)
            {
                mobileResponse.Code = 500;
                mobileResponse.Message = ex.Message;
                mobileResponse.Data = null;
                
                return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
            }
        }
        
        [AllowAnonymous]
        public virtual async Task<MobileResponseDto> IncrementOrderCounterAsync(Guid id)
        {
            var mobileResponse = new MobileResponse();
            
            try
            {
                // Get the Vacancy
                var vacancy = await _vacancyRepository.GetAsync(id);
                
                if (vacancy == null)
                {
                    mobileResponse.Code = 404;
                    mobileResponse.Message = "Vacancy not found";
                    mobileResponse.Data = null;
                    
                    return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
                }
                
                // Increment the order counter
                vacancy.OrderCounter += 1;
                
                // Update the entity
                await _vacancyRepository.UpdateAsync(vacancy);
                
                mobileResponse.Code = 200;
                mobileResponse.Message = "Order counter incremented successfully";
                mobileResponse.Data = new { 
                    VacancyId = vacancy.Id, 
                    OrderCounter = vacancy.OrderCounter 
                };
                
                return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
            }
            catch (Exception ex)
            {
                mobileResponse.Code = 500;
                mobileResponse.Message = ex.Message;
                mobileResponse.Data = null;
                
                return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
            }
        }
    }
}