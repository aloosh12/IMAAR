using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.FurnishingLevels
{
    public partial interface IFurnishingLevelRepository : IRepository<FurnishingLevel, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? name = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default);
        Task<List<FurnishingLevel>> GetListAsync(
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
            CancellationToken cancellationToken = default);
    }
}