using Imaar.VacancyAdditionalFeatures;
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
        protected IRepository<VacancyAdditionalFeature, Guid> _vacancyAdditionalFeatureRepository;

        public VacancyManagerBase(IVacancyRepository vacancyRepository,
        IRepository<VacancyAdditionalFeature, Guid> vacancyAdditionalFeatureRepository)
        {
            _vacancyRepository = vacancyRepository;
            _vacancyAdditionalFeatureRepository = vacancyAdditionalFeatureRepository;
        }

        public virtual async Task<Vacancy> CreateAsync(
        List<Guid> vacancyAdditionalFeatureIds,
        Guid serviceTypeId, Guid userProfileId, string title, string description, string location, string number, DateOnly dateOfPublish, BiologicalSex biologicalSex, int viewCounter, int orderCounter, string? latitude = null, string? longitude = null, string? expectedExperience = null, string? educationLevel = null, string? workSchedule = null, string? employmentType = null, string? languages = null, string? driveLicense = null, string? salary = null)
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
             serviceTypeId, userProfileId, title, description, location, number, dateOfPublish, biologicalSex, viewCounter, orderCounter, latitude, longitude, expectedExperience, educationLevel, workSchedule, employmentType, languages, driveLicense, salary
             );

            await SetVacancyAdditionalFeaturesAsync(vacancy, vacancyAdditionalFeatureIds);

            return await _vacancyRepository.InsertAsync(vacancy);
        }

        public virtual async Task<Vacancy> UpdateAsync(
            Guid id,
            List<Guid> vacancyAdditionalFeatureIds,
        Guid serviceTypeId, Guid userProfileId, string title, string description, string location, string number, DateOnly dateOfPublish, BiologicalSex biologicalSex, int viewCounter, int orderCounter, string? latitude = null, string? longitude = null, string? expectedExperience = null, string? educationLevel = null, string? workSchedule = null, string? employmentType = null, string? languages = null, string? driveLicense = null, string? salary = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(serviceTypeId, nameof(serviceTypeId));
            Check.NotNull(userProfileId, nameof(userProfileId));
            Check.NotNullOrWhiteSpace(title, nameof(title));
            Check.NotNullOrWhiteSpace(description, nameof(description));
            Check.NotNullOrWhiteSpace(location, nameof(location));
            Check.NotNullOrWhiteSpace(number, nameof(number));
            Check.NotNull(biologicalSex, nameof(biologicalSex));

            var queryable = await _vacancyRepository.WithDetailsAsync(x => x.VacancyAdditionalFeatures);
            var query = queryable.Where(x => x.Id == id);

            var vacancy = await AsyncExecuter.FirstOrDefaultAsync(query);

            vacancy.ServiceTypeId = serviceTypeId;
            vacancy.UserProfileId = userProfileId;
            vacancy.Title = title;
            vacancy.Description = description;
            vacancy.Location = location;
            vacancy.Number = number;
            vacancy.DateOfPublish = dateOfPublish;
            vacancy.BiologicalSex = biologicalSex;
            vacancy.ViewCounter = viewCounter;
            vacancy.OrderCounter = orderCounter;
            vacancy.Latitude = latitude;
            vacancy.Longitude = longitude;
            vacancy.ExpectedExperience = expectedExperience;
            vacancy.EducationLevel = educationLevel;
            vacancy.WorkSchedule = workSchedule;
            vacancy.EmploymentType = employmentType;
            vacancy.Languages = languages;
            vacancy.DriveLicense = driveLicense;
            vacancy.Salary = salary;

            await SetVacancyAdditionalFeaturesAsync(vacancy, vacancyAdditionalFeatureIds);

            vacancy.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _vacancyRepository.UpdateAsync(vacancy);
        }

        private async Task SetVacancyAdditionalFeaturesAsync(Vacancy vacancy, List<Guid> vacancyAdditionalFeatureIds)
        {
            if (vacancyAdditionalFeatureIds == null || !vacancyAdditionalFeatureIds.Any())
            {
                vacancy.RemoveAllVacancyAdditionalFeatures();
                return;
            }

            var query = (await _vacancyAdditionalFeatureRepository.GetQueryableAsync())
                .Where(x => vacancyAdditionalFeatureIds.Contains(x.Id))
                .Select(x => x.Id);

            var vacancyAdditionalFeatureIdsInDb = await AsyncExecuter.ToListAsync(query);
            if (!vacancyAdditionalFeatureIdsInDb.Any())
            {
                return;
            }

            vacancy.RemoveAllVacancyAdditionalFeaturesExceptGivenIds(vacancyAdditionalFeatureIdsInDb);

            foreach (var vacancyAdditionalFeatureId in vacancyAdditionalFeatureIdsInDb)
            {
                vacancy.AddVacancyAdditionalFeature(vacancyAdditionalFeatureId);
            }
        }

    }
}