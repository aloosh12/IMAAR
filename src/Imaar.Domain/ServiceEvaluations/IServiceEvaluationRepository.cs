using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.ServiceEvaluations
{
    public partial interface IServiceEvaluationRepository : IRepository<ServiceEvaluation, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            Guid? evaluatorId = null,
            Guid? imaarServiceId = null,
            CancellationToken cancellationToken = default);
        Task<ServiceEvaluationWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<ServiceEvaluationWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            Guid? evaluatorId = null,
            Guid? imaarServiceId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<ServiceEvaluation>> GetListAsync(
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
            Guid? imaarServiceId = null,
            CancellationToken cancellationToken = default);
    }
}