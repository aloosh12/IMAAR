using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.BuildingEvaluations
{
    public partial interface IBuildingEvaluationRepository : IRepository<BuildingEvaluation, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            Guid? evaluatorId = null,
            Guid? buildingId = null,
            CancellationToken cancellationToken = default);
        Task<BuildingEvaluationWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<BuildingEvaluationWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            Guid? evaluatorId = null,
            Guid? buildingId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<BuildingEvaluation>> GetListAsync(
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
            Guid? evaluatorId = null,
            Guid? buildingId = null,
            CancellationToken cancellationToken = default);
    }
}