using Imaar.Vacancies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.Vacancies
{
    public abstract class VacancyManagerBase : DomainService
    {
        protected IVacancyRepository _vacancyRepository;

        public VacancyManagerBase(IVacancyRepository vacancyRepository)
        {
            _vacancyRepository = vacancyRepository;
        }

        public virtual async Task<Vacancy> CreateAsync(
        Guid serviceTypeId, Guid userProfileId, string title, string description, string location, string number, DateOnly dateOfPublish, BiologicalSex biologicalSex, string? latitude = null, string? longitude = null, string? expectedExperience = null, string? educationLevel = null, string? workSchedule = null, string? employmentType = null, string? languages = null, string? driveLicense = null, string? salary = null)
        {
            Check.NotNull(serviceTypeId, nameof(serviceTypeId));
            Check.NotNull(userProfileId, nameof(userProfileId));
            Check.NotNullOrWhiteSpace(title, nameof(title));
            Check.NotNullOrWhiteSpace(description, nameof(description));
            Check.NotNullOrWhiteSpace(location, nameof(location));
            Check.NotNullOrWhiteSpace(number, nameof(number));
            Check.NotNull(biologicalSex, nameof(biologicalSex));

            var vacancy = new Vacancy(
             GuidGenerator.Create(),
             serviceTypeId, userProfileId, title, description, location, number, dateOfPublish, biologicalSex, latitude, longitude, expectedExperience, educationLevel, workSchedule, employmentType, languages, driveLicense, salary
             );

            return await _vacancyRepository.InsertAsync(vacancy);
        }

        public virtual async Task<Vacancy> UpdateAsync(
            Guid id,
            Guid serviceTypeId, Guid userProfileId, string title, string description, string location, string number, DateOnly dateOfPublish, BiologicalSex biologicalSex, string? latitude = null, string? longitude = null, string? expectedExperience = null, string? educationLevel = null, string? workSchedule = null, string? employmentType = null, string? languages = null, string? driveLicense = null, string? salary = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(serviceTypeId, nameof(serviceTypeId));
            Check.NotNull(userProfileId, nameof(userProfileId));
            Check.NotNullOrWhiteSpace(title, nameof(title));
            Check.NotNullOrWhiteSpace(description, nameof(description));
            Check.NotNullOrWhiteSpace(location, nameof(location));
            Check.NotNullOrWhiteSpace(number, nameof(number));
            Check.NotNull(biologicalSex, nameof(biologicalSex));

            var vacancy = await _vacancyRepository.GetAsync(id);

            vacancy.ServiceTypeId = serviceTypeId;
            vacancy.UserProfileId = userProfileId;
            vacancy.Title = title;
            vacancy.Description = description;
            vacancy.Location = location;
            vacancy.Number = number;
            vacancy.DateOfPublish = dateOfPublish;
            vacancy.BiologicalSex = biologicalSex;
            vacancy.Latitude = latitude;
            vacancy.Longitude = longitude;
            vacancy.ExpectedExperience = expectedExperience;
            vacancy.EducationLevel = educationLevel;
            vacancy.WorkSchedule = workSchedule;
            vacancy.EmploymentType = employmentType;
            vacancy.Languages = languages;
            vacancy.DriveLicense = driveLicense;
            vacancy.Salary = salary;

            vacancy.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _vacancyRepository.UpdateAsync(vacancy);
        }

    }
}