using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.UserEvalauations
{
    public partial interface IUserEvalauationRepository : IRepository<UserEvalauation, Guid>
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
        Task<UserEvalauationWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<UserEvalauationWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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

        Task<List<UserEvalauation>> GetListAsync(
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