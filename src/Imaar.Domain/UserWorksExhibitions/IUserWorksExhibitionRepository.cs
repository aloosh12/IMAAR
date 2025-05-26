using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.UserWorksExhibitions
{
    public partial interface IUserWorksExhibitionRepository : IRepository<UserWorksExhibition, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? title = null,
            string? file = null,
            int? orderMin = null,
            int? orderMax = null,
            Guid? userProfileId = null,
            CancellationToken cancellationToken = default);
        Task<UserWorksExhibitionWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<UserWorksExhibitionWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? title = null,
            string? file = null,
            int? orderMin = null,
            int? orderMax = null,
            Guid? userProfileId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<UserWorksExhibition>> GetListAsync(
                    string? filterText = null,
                    string? title = null,
                    string? file = null,
                    int? orderMin = null,
                    int? orderMax = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? title = null,
            string? file = null,
            int? orderMin = null,
            int? orderMax = null,
            Guid? userProfileId = null,
            CancellationToken cancellationToken = default);
    }
}