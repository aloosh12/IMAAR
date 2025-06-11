using Imaar.ServiceTypes;
using Imaar.UserProfiles;
using Imaar.Vacancies;
using Imaar.VacancyAdditionalFeatures;
using Imaar.EntityFrameworkCore;
using Imaar.ServiceTypes;
using Imaar.UserProfiles;
using Imaar.Vacancies;
using Imaar.VacancyAdditionalFeatures;
using Imaar.VacancyAdditionalFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Imaar.Vacancies
{
    public abstract class EfCoreVacancyRepositoryBase : EfCoreRepository<ImaarDbContext, Vacancy, Guid>
    {
        public EfCoreVacancyRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? title = null,
            string? description = null,
            string? location = null,
            string? number = null,
            string? latitude = null,
            string? longitude = null,
            DateOnly? dateOfPublishMin = null,
            DateOnly? dateOfPublishMax = null,
            string? expectedExperience = null,
            string? educationLevel = null,
            string? workSchedule = null,
            string? employmentType = null,
            BiologicalSex? biologicalSex = null,
            string? languages = null,
            string? driveLicense = null,
            string? salary = null,
            string? phoneNumber = null,
            int? viewCounterMin = null,
            int? viewCounterMax = null,
            int? orderCounterMin = null,
            int? orderCounterMax = null,
            Guid? serviceTypeId = null,
            Guid? userProfileId = null,
            Guid? vacancyAdditionalFeatureId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, title, description, location, number, latitude, longitude, dateOfPublishMin, dateOfPublishMax, expectedExperience, educationLevel, workSchedule, employmentType, biologicalSex, languages, driveLicense, salary, phoneNumber, viewCounterMin, viewCounterMax, orderCounterMin, orderCounterMax, serviceTypeId, userProfileId, vacancyAdditionalFeatureId);

            var ids = query.Select(x => x.Vacancy.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<VacancyWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id).Include(x => x.VacancyAdditionalFeatures)
                .Select(vacancy => new VacancyWithNavigationProperties
                {
                    Vacancy = vacancy,
                    ServiceType = dbContext.Set<ServiceType>().FirstOrDefault(c => c.Id == vacancy.ServiceTypeId),
                    UserProfile = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == vacancy.UserProfileId),
                    VacancyAdditionalFeatures = (from vacancyVacancyAdditionalFeatures in vacancy.VacancyAdditionalFeatures
                                                 join _vacancyAdditionalFeature in dbContext.Set<VacancyAdditionalFeature>() on vacancyVacancyAdditionalFeatures.VacancyAdditionalFeatureId equals _vacancyAdditionalFeature.Id
                                                 select _vacancyAdditionalFeature).ToList()
                }).FirstOrDefault();
        }

        public virtual async Task<List<VacancyWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? title = null,
            string? description = null,
            string? location = null,
            string? number = null,
            string? latitude = null,
            string? longitude = null,
            DateOnly? dateOfPublishMin = null,
            DateOnly? dateOfPublishMax = null,
            string? expectedExperience = null,
            string? educationLevel = null,
            string? workSchedule = null,
            string? employmentType = null,
            BiologicalSex? biologicalSex = null,
            string? languages = null,
            string? driveLicense = null,
            string? salary = null,
            string? phoneNumber = null,
            int? viewCounterMin = null,
            int? viewCounterMax = null,
            int? orderCounterMin = null,
            int? orderCounterMax = null,
            Guid? serviceTypeId = null,
            Guid? userProfileId = null,
            Guid? vacancyAdditionalFeatureId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, title, description, location, number, latitude, longitude, dateOfPublishMin, dateOfPublishMax, expectedExperience, educationLevel, workSchedule, employmentType, biologicalSex, languages, driveLicense, salary, phoneNumber, viewCounterMin, viewCounterMax, orderCounterMin, orderCounterMax, serviceTypeId, userProfileId, vacancyAdditionalFeatureId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? VacancyConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<VacancyWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from vacancy in (await GetDbSetAsync())
                   join serviceType in (await GetDbContextAsync()).Set<ServiceType>() on vacancy.ServiceTypeId equals serviceType.Id into serviceTypes
                   from serviceType in serviceTypes.DefaultIfEmpty()
                   join userProfile in (await GetDbContextAsync()).Set<UserProfile>() on vacancy.UserProfileId equals userProfile.Id into userProfiles
                   from userProfile in userProfiles.DefaultIfEmpty()
                   select new VacancyWithNavigationProperties
                   {
                       Vacancy = vacancy,
                       ServiceType = serviceType,
                       UserProfile = userProfile,
                       VacancyAdditionalFeatures = new List<VacancyAdditionalFeature>()
                   };
        }

        protected virtual IQueryable<VacancyWithNavigationProperties> ApplyFilter(
            IQueryable<VacancyWithNavigationProperties> query,
            string? filterText,
            string? title = null,
            string? description = null,
            string? location = null,
            string? number = null,
            string? latitude = null,
            string? longitude = null,
            DateOnly? dateOfPublishMin = null,
            DateOnly? dateOfPublishMax = null,
            string? expectedExperience = null,
            string? educationLevel = null,
            string? workSchedule = null,
            string? employmentType = null,
            BiologicalSex? biologicalSex = null,
            string? languages = null,
            string? driveLicense = null,
            string? salary = null,
            string? phoneNumber = null,
            int? viewCounterMin = null,
            int? viewCounterMax = null,
            int? orderCounterMin = null,
            int? orderCounterMax = null,
            Guid? serviceTypeId = null,
            Guid? userProfileId = null,
            Guid? vacancyAdditionalFeatureId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Vacancy.Title!.Contains(filterText!) || e.Vacancy.Description!.Contains(filterText!) || e.Vacancy.Location!.Contains(filterText!) || e.Vacancy.Number!.Contains(filterText!) || e.Vacancy.Latitude!.Contains(filterText!) || e.Vacancy.Longitude!.Contains(filterText!) || e.Vacancy.ExpectedExperience!.Contains(filterText!) || e.Vacancy.EducationLevel!.Contains(filterText!) || e.Vacancy.WorkSchedule!.Contains(filterText!) || e.Vacancy.EmploymentType!.Contains(filterText!) || e.Vacancy.Languages!.Contains(filterText!) || e.Vacancy.DriveLicense!.Contains(filterText!) || e.Vacancy.Salary!.Contains(filterText!) || e.Vacancy.PhoneNumber!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(title), e => e.Vacancy.Title.Contains(title))
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Vacancy.Description.Contains(description))
                    .WhereIf(!string.IsNullOrWhiteSpace(location), e => e.Vacancy.Location.Contains(location))
                    .WhereIf(!string.IsNullOrWhiteSpace(number), e => e.Vacancy.Number.Contains(number))
                    .WhereIf(!string.IsNullOrWhiteSpace(latitude), e => e.Vacancy.Latitude.Contains(latitude))
                    .WhereIf(!string.IsNullOrWhiteSpace(longitude), e => e.Vacancy.Longitude.Contains(longitude))
                    .WhereIf(dateOfPublishMin.HasValue, e => e.Vacancy.DateOfPublish >= dateOfPublishMin!.Value)
                    .WhereIf(dateOfPublishMax.HasValue, e => e.Vacancy.DateOfPublish <= dateOfPublishMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(expectedExperience), e => e.Vacancy.ExpectedExperience.Contains(expectedExperience))
                    .WhereIf(!string.IsNullOrWhiteSpace(educationLevel), e => e.Vacancy.EducationLevel.Contains(educationLevel))
                    .WhereIf(!string.IsNullOrWhiteSpace(workSchedule), e => e.Vacancy.WorkSchedule.Contains(workSchedule))
                    .WhereIf(!string.IsNullOrWhiteSpace(employmentType), e => e.Vacancy.EmploymentType.Contains(employmentType))
                    .WhereIf(biologicalSex.HasValue, e => e.Vacancy.BiologicalSex == biologicalSex)
                    .WhereIf(!string.IsNullOrWhiteSpace(languages), e => e.Vacancy.Languages.Contains(languages))
                    .WhereIf(!string.IsNullOrWhiteSpace(driveLicense), e => e.Vacancy.DriveLicense.Contains(driveLicense))
                    .WhereIf(!string.IsNullOrWhiteSpace(salary), e => e.Vacancy.Salary.Contains(salary))
                    .WhereIf(!string.IsNullOrWhiteSpace(phoneNumber), e => e.Vacancy.PhoneNumber.Contains(phoneNumber))
                    .WhereIf(viewCounterMin.HasValue, e => e.Vacancy.ViewCounter >= viewCounterMin!.Value)
                    .WhereIf(viewCounterMax.HasValue, e => e.Vacancy.ViewCounter <= viewCounterMax!.Value)
                    .WhereIf(orderCounterMin.HasValue, e => e.Vacancy.OrderCounter >= orderCounterMin!.Value)
                    .WhereIf(orderCounterMax.HasValue, e => e.Vacancy.OrderCounter <= orderCounterMax!.Value)
                    .WhereIf(serviceTypeId != null && serviceTypeId != Guid.Empty, e => e.ServiceType != null && e.ServiceType.Id == serviceTypeId)
                    .WhereIf(userProfileId != null && userProfileId != Guid.Empty, e => e.UserProfile != null && e.UserProfile.Id == userProfileId)
                    .WhereIf(vacancyAdditionalFeatureId != null && vacancyAdditionalFeatureId != Guid.Empty, e => e.Vacancy.VacancyAdditionalFeatures.Any(x => x.VacancyAdditionalFeatureId == vacancyAdditionalFeatureId));
        }

        public virtual async Task<List<Vacancy>> GetListAsync(
            string? filterText = null,
            string? title = null,
            string? description = null,
            string? location = null,
            string? number = null,
            string? latitude = null,
            string? longitude = null,
            DateOnly? dateOfPublishMin = null,
            DateOnly? dateOfPublishMax = null,
            string? expectedExperience = null,
            string? educationLevel = null,
            string? workSchedule = null,
            string? employmentType = null,
            BiologicalSex? biologicalSex = null,
            string? languages = null,
            string? driveLicense = null,
            string? salary = null,
            string? phoneNumber = null,
            int? viewCounterMin = null,
            int? viewCounterMax = null,
            int? orderCounterMin = null,
            int? orderCounterMax = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, title, description, location, number, latitude, longitude, dateOfPublishMin, dateOfPublishMax, expectedExperience, educationLevel, workSchedule, employmentType, biologicalSex, languages, driveLicense, salary, phoneNumber, viewCounterMin, viewCounterMax, orderCounterMin, orderCounterMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? VacancyConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? title = null,
            string? description = null,
            string? location = null,
            string? number = null,
            string? latitude = null,
            string? longitude = null,
            DateOnly? dateOfPublishMin = null,
            DateOnly? dateOfPublishMax = null,
            string? expectedExperience = null,
            string? educationLevel = null,
            string? workSchedule = null,
            string? employmentType = null,
            BiologicalSex? biologicalSex = null,
            string? languages = null,
            string? driveLicense = null,
            string? salary = null,
            string? phoneNumber = null,
            int? viewCounterMin = null,
            int? viewCounterMax = null,
            int? orderCounterMin = null,
            int? orderCounterMax = null,
            Guid? serviceTypeId = null,
            Guid? userProfileId = null,
            Guid? vacancyAdditionalFeatureId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, title, description, location, number, latitude, longitude, dateOfPublishMin, dateOfPublishMax, expectedExperience, educationLevel, workSchedule, employmentType, biologicalSex, languages, driveLicense, salary, phoneNumber, viewCounterMin, viewCounterMax, orderCounterMin, orderCounterMax, serviceTypeId, userProfileId, vacancyAdditionalFeatureId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Vacancy> ApplyFilter(
            IQueryable<Vacancy> query,
            string? filterText = null,
            string? title = null,
            string? description = null,
            string? location = null,
            string? number = null,
            string? latitude = null,
            string? longitude = null,
            DateOnly? dateOfPublishMin = null,
            DateOnly? dateOfPublishMax = null,
            string? expectedExperience = null,
            string? educationLevel = null,
            string? workSchedule = null,
            string? employmentType = null,
            BiologicalSex? biologicalSex = null,
            string? languages = null,
            string? driveLicense = null,
            string? salary = null,
            string? phoneNumber = null,
            int? viewCounterMin = null,
            int? viewCounterMax = null,
            int? orderCounterMin = null,
            int? orderCounterMax = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Title!.Contains(filterText!) || e.Description!.Contains(filterText!) || e.Location!.Contains(filterText!) || e.Number!.Contains(filterText!) || e.Latitude!.Contains(filterText!) || e.Longitude!.Contains(filterText!) || e.ExpectedExperience!.Contains(filterText!) || e.EducationLevel!.Contains(filterText!) || e.WorkSchedule!.Contains(filterText!) || e.EmploymentType!.Contains(filterText!) || e.Languages!.Contains(filterText!) || e.DriveLicense!.Contains(filterText!) || e.Salary!.Contains(filterText!) || e.PhoneNumber!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(title), e => e.Title.Contains(title))
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description))
                    .WhereIf(!string.IsNullOrWhiteSpace(location), e => e.Location.Contains(location))
                    .WhereIf(!string.IsNullOrWhiteSpace(number), e => e.Number.Contains(number))
                    .WhereIf(!string.IsNullOrWhiteSpace(latitude), e => e.Latitude.Contains(latitude))
                    .WhereIf(!string.IsNullOrWhiteSpace(longitude), e => e.Longitude.Contains(longitude))
                    .WhereIf(dateOfPublishMin.HasValue, e => e.DateOfPublish >= dateOfPublishMin!.Value)
                    .WhereIf(dateOfPublishMax.HasValue, e => e.DateOfPublish <= dateOfPublishMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(expectedExperience), e => e.ExpectedExperience.Contains(expectedExperience))
                    .WhereIf(!string.IsNullOrWhiteSpace(educationLevel), e => e.EducationLevel.Contains(educationLevel))
                    .WhereIf(!string.IsNullOrWhiteSpace(workSchedule), e => e.WorkSchedule.Contains(workSchedule))
                    .WhereIf(!string.IsNullOrWhiteSpace(employmentType), e => e.EmploymentType.Contains(employmentType))
                    .WhereIf(biologicalSex.HasValue, e => e.BiologicalSex == biologicalSex)
                    .WhereIf(!string.IsNullOrWhiteSpace(languages), e => e.Languages.Contains(languages))
                    .WhereIf(!string.IsNullOrWhiteSpace(driveLicense), e => e.DriveLicense.Contains(driveLicense))
                    .WhereIf(!string.IsNullOrWhiteSpace(salary), e => e.Salary.Contains(salary))
                    .WhereIf(!string.IsNullOrWhiteSpace(phoneNumber), e => e.PhoneNumber.Contains(phoneNumber))
                    .WhereIf(viewCounterMin.HasValue, e => e.ViewCounter >= viewCounterMin!.Value)
                    .WhereIf(viewCounterMax.HasValue, e => e.ViewCounter <= viewCounterMax!.Value)
                    .WhereIf(orderCounterMin.HasValue, e => e.OrderCounter >= orderCounterMin!.Value)
                    .WhereIf(orderCounterMax.HasValue, e => e.OrderCounter <= orderCounterMax!.Value);
        }
    }
}