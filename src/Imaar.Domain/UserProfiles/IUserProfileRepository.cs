using Imaar.UserProfiles;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.UserProfiles
{
    public partial interface IUserProfileRepository : IRepository<UserProfile, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? securityNumber = null,
            BiologicalSex? biologicalSex = null,
            DateOnly? dateOfBirthMin = null,
            DateOnly? dateOfBirthMax = null,
            string? latitude = null,
            string? longitude = null,
            CancellationToken cancellationToken = default);
        Task<List<UserProfile>> GetListAsync(
                    string? filterText = null,
                    string? securityNumber = null,
                    BiologicalSex? biologicalSex = null,
                    DateOnly? dateOfBirthMin = null,
                    DateOnly? dateOfBirthMax = null,
                    string? latitude = null,
                    string? longitude = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? securityNumber = null,
            BiologicalSex? biologicalSex = null,
            DateOnly? dateOfBirthMin = null,
            DateOnly? dateOfBirthMax = null,
            string? latitude = null,
            string? longitude = null,
            CancellationToken cancellationToken = default);
    }
}