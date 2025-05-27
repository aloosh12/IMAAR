using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.Regions
{
    public partial interface IRegionRepository : IRepository<Region, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? name = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? cityId = null,
            CancellationToken cancellationToken = default);
        Task<RegionWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<RegionWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? name = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? cityId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<Region>> GetListAsync(
                    string? filterText = null,
                    string? name = null,
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
            string? name = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? cityId = null,
            CancellationToken cancellationToken = default);
    }
}