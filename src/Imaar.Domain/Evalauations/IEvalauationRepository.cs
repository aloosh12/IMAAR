using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.Evalauations
{
    public partial interface IEvalauationRepository : IRepository<Evalauation, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            int? speedOfCompletionMin = null,
            int? speedOfCompletionMax = null,
            int? dealingMin = null,
            int? dealingMax = null,
            int? cleanlinessMin = null,
            int? cleanlinessMax = null,
            int? perfectionMin = null,
            int? perfectionMax = null,
            int? priceMin = null,
            int? priceMax = null,
            Guid? evaluatord = null,
            Guid? evaluatedPersonId = null,
            CancellationToken cancellationToken = default);
        Task<EvalauationWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<EvalauationWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            int? speedOfCompletionMin = null,
            int? speedOfCompletionMax = null,
            int? dealingMin = null,
            int? dealingMax = null,
            int? cleanlinessMin = null,
            int? cleanlinessMax = null,
            int? perfectionMin = null,
            int? perfectionMax = null,
            int? priceMin = null,
            int? priceMax = null,
            Guid? evaluatord = null,
            Guid? evaluatedPersonId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<Evalauation>> GetListAsync(
                    string? filterText = null,
                    int? speedOfCompletionMin = null,
                    int? speedOfCompletionMax = null,
                    int? dealingMin = null,
                    int? dealingMax = null,
                    int? cleanlinessMin = null,
                    int? cleanlinessMax = null,
                    int? perfectionMin = null,
                    int? perfectionMax = null,
                    int? priceMin = null,
                    int? priceMax = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            int? speedOfCompletionMin = null,
            int? speedOfCompletionMax = null,
            int? dealingMin = null,
            int? dealingMax = null,
            int? cleanlinessMin = null,
            int? cleanlinessMax = null,
            int? perfectionMin = null,
            int? perfectionMax = null,
            int? priceMin = null,
            int? priceMax = null,
            Guid? evaluatord = null,
            Guid? evaluatedPersonId = null,
            CancellationToken cancellationToken = default);
    }
}