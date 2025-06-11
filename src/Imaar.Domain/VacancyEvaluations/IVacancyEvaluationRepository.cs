using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.VacancyEvaluations
{
    public partial interface IVacancyEvaluationRepository : IRepository<VacancyEvaluation, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            Guid? userProfileId = null,
            Guid? vacancyId = null,
            CancellationToken cancellationToken = default);
        Task<VacancyEvaluationWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<VacancyEvaluationWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            Guid? userProfileId = null,
            Guid? vacancyId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<VacancyEvaluation>> GetListAsync(
                    string? filterText = null,
                    int? rateMin = null,
                    int? rateMax = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            Guid? userProfileId = null,
            Guid? vacancyId = null,
            CancellationToken cancellationToken = default);
    }
}