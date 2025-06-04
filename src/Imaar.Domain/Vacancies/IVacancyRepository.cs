using Imaar.Vacancies;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.Vacancies
{
    public partial interface IVacancyRepository : IRepository<Vacancy, Guid>
    {

        Task DeleteAllAsync(
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
            int? viewCounterMin = null,
            int? viewCounterMax = null,
            int? orderCounterMin = null,
            int? orderCounterMax = null,
            Guid? serviceTypeId = null,
            Guid? userProfileId = null,
            CancellationToken cancellationToken = default);
        Task<VacancyWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<VacancyWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
            int? viewCounterMin = null,
            int? viewCounterMax = null,
            int? orderCounterMin = null,
            int? orderCounterMax = null,
            Guid? serviceTypeId = null,
            Guid? userProfileId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<Vacancy>> GetListAsync(
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
                    int? viewCounterMin = null,
                    int? viewCounterMax = null,
                    int? orderCounterMin = null,
                    int? orderCounterMax = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
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
            int? viewCounterMin = null,
            int? viewCounterMax = null,
            int? orderCounterMin = null,
            int? orderCounterMax = null,
            Guid? serviceTypeId = null,
            Guid? userProfileId = null,
            CancellationToken cancellationToken = default);
    }
}