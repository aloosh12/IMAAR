using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.Medias
{
    public partial interface IMediaRepository : IRepository<Media, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? title = null,
            string? file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? imaarServiceId = null,
            Guid? vacancyId = null,
            Guid? storyId = null,
            CancellationToken cancellationToken = default);
        Task<MediaWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<MediaWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? title = null,
            string? file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? imaarServiceId = null,
            Guid? vacancyId = null,
            Guid? storyId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<Media>> GetListAsync(
                    string? filterText = null,
                    string? title = null,
                    string? file = null,
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
            string? file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? imaarServiceId = null,
            Guid? vacancyId = null,
            Guid? storyId = null,
            CancellationToken cancellationToken = default);
    }
}