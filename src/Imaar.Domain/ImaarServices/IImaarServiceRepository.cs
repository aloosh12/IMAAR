using Imaar.ImaarServices;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.ImaarServices
{
    public partial interface IImaarServiceRepository : IRepository<ImaarService, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? title = null,
            string? description = null,
            string? serviceLocation = null,
            string? serviceNumber = null,
            DateOnly? dateOfPublishMin = null,
            DateOnly? dateOfPublishMax = null,
            int? priceMin = null,
            int? priceMax = null,
            string? latitude = null,
            string? longitude = null,
            string? phoneNumber = null,
            int? viewCounterMin = null,
            int? viewCounterMax = null,
            int? orderCounterMin = null,
            int? orderCounterMax = null,
            Guid? serviceTypeId = null,
            Guid? userProfileId = null,
            CancellationToken cancellationToken = default);
        Task<ImaarServiceWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<ImaarServiceWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? title = null,
            string? description = null,
            string? serviceLocation = null,
            string? serviceNumber = null,
            DateOnly? dateOfPublishMin = null,
            DateOnly? dateOfPublishMax = null,
            int? priceMin = null,
            int? priceMax = null,
            string? latitude = null,
            string? longitude = null,
            string? phoneNumber = null,
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

        Task<List<ImaarService>> GetListAsync(
                    string? filterText = null,
                    string? title = null,
                    string? description = null,
                    string? serviceLocation = null,
                    string? serviceNumber = null,
                    DateOnly? dateOfPublishMin = null,
                    DateOnly? dateOfPublishMax = null,
                    int? priceMin = null,
                    int? priceMax = null,
                    string? latitude = null,
                    string? longitude = null,
                    string? phoneNumber = null,
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
            string? serviceLocation = null,
            string? serviceNumber = null,
            DateOnly? dateOfPublishMin = null,
            DateOnly? dateOfPublishMax = null,
            int? priceMin = null,
            int? priceMax = null,
            string? latitude = null,
            string? longitude = null,
            string? phoneNumber = null,
            int? viewCounterMin = null,
            int? viewCounterMax = null,
            int? orderCounterMin = null,
            int? orderCounterMax = null,
            Guid? serviceTypeId = null,
            Guid? userProfileId = null,
            CancellationToken cancellationToken = default);
    }
}