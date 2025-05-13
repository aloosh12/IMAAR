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

namespace Imaar.Vacancies
{

    [Authorize(ImaarPermissions.Vacancies.Default)]
    public abstract class VacanciesAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<VacancyDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IVacancyRepository _vacancyRepository;
        protected VacancyManager _vacancyManager;

        protected IRepository<Imaar.ServiceTypes.ServiceType, Guid> _serviceTypeRepository;
        protected IRepository<Imaar.UserProfiles.UserProfile, Guid> _userProfileRepository;

        public VacanciesAppServiceBase(IVacancyRepository vacancyRepository, VacancyManager vacancyManager, IDistributedCache<VacancyDownloadTokenCacheItem, string> downloadTokenCache, IRepository<Imaar.ServiceTypes.ServiceType, Guid> serviceTypeRepository, IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _vacancyRepository = vacancyRepository;
            _vacancyManager = vacancyManager; _serviceTypeRepository = serviceTypeRepository;
            _userProfileRepository = userProfileRepository;

        }

        public virtual async Task<PagedResultDto<VacancyWithNavigationPropertiesDto>> GetListAsync(GetVacanciesInput input)
        {
            var totalCount = await _vacancyRepository.GetCountAsync(input.FilterText, input.Title, input.Description, input.Location, input.Number, input.Latitude, input.Longitude, input.DateOfPublishMin, input.DateOfPublishMax, input.ExpectedExperience, input.EducationLevel, input.WorkSchedule, input.EmploymentType, input.BiologicalSex, input.Languages, input.DriveLicense, input.Salary, input.ServiceTypeId, input.UserProfileId);
            var items = await _vacancyRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Title, input.Description, input.Location, input.Number, input.Latitude, input.Longitude, input.DateOfPublishMin, input.DateOfPublishMax, input.ExpectedExperience, input.EducationLevel, input.WorkSchedule, input.EmploymentType, input.BiologicalSex, input.Languages, input.DriveLicense, input.Salary, input.ServiceTypeId, input.UserProfileId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<VacancyWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<VacancyWithNavigationProperties>, List<VacancyWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<VacancyWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<VacancyWithNavigationProperties, VacancyWithNavigationPropertiesDto>
                (await _vacancyRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<VacancyDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Vacancy, VacancyDto>(await _vacancyRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetServiceTypeLookupAsync(LookupRequestDto input)
        {
            var query = (await _serviceTypeRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Title != null &&
                         x.Title.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.ServiceTypes.ServiceType>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.ServiceTypes.ServiceType>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input)
        {
            var query = (await _userProfileRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.SecurityNumber != null &&
                         x.SecurityNumber.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.UserProfiles.UserProfile>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.UserProfiles.UserProfile>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(ImaarPermissions.Vacancies.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _vacancyRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.Vacancies.Create)]
        public virtual async Task<VacancyDto> CreateAsync(VacancyCreateDto input)
        {
            if (input.ServiceTypeId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["ServiceType"]]);
            }
            if (input.UserProfileId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }

            var vacancy = await _vacancyManager.CreateAsync(
            input.ServiceTypeId, input.UserProfileId, input.Title, input.Description, input.Location, input.Number, input.DateOfPublish, input.BiologicalSex, input.Latitude, input.Longitude, input.ExpectedExperience, input.EducationLevel, input.WorkSchedule, input.EmploymentType, input.Languages, input.DriveLicense, input.Salary
            );

            return ObjectMapper.Map<Vacancy, VacancyDto>(vacancy);
        }

        [Authorize(ImaarPermissions.Vacancies.Edit)]
        public virtual async Task<VacancyDto> UpdateAsync(Guid id, VacancyUpdateDto input)
        {
            if (input.ServiceTypeId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["ServiceType"]]);
            }
            if (input.UserProfileId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }

            var vacancy = await _vacancyManager.UpdateAsync(
            id,
            input.ServiceTypeId, input.UserProfileId, input.Title, input.Description, input.Location, input.Number, input.DateOfPublish, input.BiologicalSex, input.Latitude, input.Longitude, input.ExpectedExperience, input.EducationLevel, input.WorkSchedule, input.EmploymentType, input.Languages, input.DriveLicense, input.Salary, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Vacancy, VacancyDto>(vacancy);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(VacancyExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var vacancies = await _vacancyRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Title, input.Description, input.Location, input.Number, input.Latitude, input.Longitude, input.DateOfPublishMin, input.DateOfPublishMax, input.ExpectedExperience, input.EducationLevel, input.WorkSchedule, input.EmploymentType, input.BiologicalSex, input.Languages, input.DriveLicense, input.Salary, input.ServiceTypeId, input.UserProfileId);
            var items = vacancies.Select(item => new
            {
                Title = item.Vacancy.Title,
                Description = item.Vacancy.Description,
                Location = item.Vacancy.Location,
                Number = item.Vacancy.Number,
                Latitude = item.Vacancy.Latitude,
                Longitude = item.Vacancy.Longitude,
                DateOfPublish = item.Vacancy.DateOfPublish,
                ExpectedExperience = item.Vacancy.ExpectedExperience,
                EducationLevel = item.Vacancy.EducationLevel,
                WorkSchedule = item.Vacancy.WorkSchedule,
                EmploymentType = item.Vacancy.EmploymentType,
                BiologicalSex = item.Vacancy.BiologicalSex,
                Languages = item.Vacancy.Languages,
                DriveLicense = item.Vacancy.DriveLicense,
                Salary = item.Vacancy.Salary,

                ServiceType = item.ServiceType?.Title,
                UserProfile = item.UserProfile?.SecurityNumber,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Vacancies.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.Vacancies.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> vacancyIds)
        {
            await _vacancyRepository.DeleteManyAsync(vacancyIds);
        }

        [Authorize(ImaarPermissions.Vacancies.Delete)]
        public virtual async Task DeleteAllAsync(GetVacanciesInput input)
        {
            await _vacancyRepository.DeleteAllAsync(input.FilterText, input.Title, input.Description, input.Location, input.Number, input.Latitude, input.Longitude, input.DateOfPublishMin, input.DateOfPublishMax, input.ExpectedExperience, input.EducationLevel, input.WorkSchedule, input.EmploymentType, input.BiologicalSex, input.Languages, input.DriveLicense, input.Salary, input.ServiceTypeId, input.UserProfileId);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new VacancyDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new Imaar.Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}