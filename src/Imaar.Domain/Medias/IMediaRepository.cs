using Imaar.Medias;
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
            MediaEntityType? sourceEntityType = null,
            string? sourceEntityId = null,
            CancellationToken cancellationToken = default);
        Task<List<Media>> GetListAsync(
                    string? filterText = null,
                    string? title = null,
                    string? file = null,
                    int? orderMin = null,
                    int? orderMax = null,
                    bool? isActive = null,
                    MediaEntityType? sourceEntityType = null,
                    string? sourceEntityId = null,
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
            MediaEntityType? sourceEntityType = null,
            string? sourceEntityId = null,
            CancellationToken cancellationToken = default);
    }
}