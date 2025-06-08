using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.Advertisements
{
    public partial interface IAdvertisementRepository : IRepository<Advertisement, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? title = null,
            string? subTitle = null,
            string? file = null,
            DateTime? fromDateTimeMin = null,
            DateTime? fromDateTimeMax = null,
            DateTime? toDateTimeMin = null,
            DateTime? toDateTimeMax = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? userProfileId = null,
            CancellationToken cancellationToken = default);
        Task<AdvertisementWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<AdvertisementWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? title = null,
            string? subTitle = null,
            string? file = null,
            DateTime? fromDateTimeMin = null,
            DateTime? fromDateTimeMax = null,
            DateTime? toDateTimeMin = null,
            DateTime? toDateTimeMax = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? userProfileId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<Advertisement>> GetListAsync(
                    string? filterText = null,
                    string? title = null,
                    string? subTitle = null,
                    string? file = null,
                    DateTime? fromDateTimeMin = null,
                    DateTime? fromDateTimeMax = null,
                    DateTime? toDateTimeMin = null,
                    DateTime? toDateTimeMax = null,
                    int? orderMin = null,
                    int? orderMax = null,
                    bool? isActive = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? title = null,
            string? subTitle = null,
            string? file = null,
            DateTime? fromDateTimeMin = null,
            DateTime? fromDateTimeMax = null,
            DateTime? toDateTimeMin = null,
            DateTime? toDateTimeMax = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? userProfileId = null,
            CancellationToken cancellationToken = default);
    }
}