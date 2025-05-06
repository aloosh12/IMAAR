using Imaar.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Imaar.EntityFrameworkCore;

namespace Imaar.UserProfiles
{
    public abstract class EfCoreUserProfileRepositoryBase : EfCoreRepository<ImaarDbContext, UserProfile, Guid>
    {
        public EfCoreUserProfileRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? securityNumber = null,
            BiologicalSex? biologicalSex = null,
            DateOnly? dateOfBirthMin = null,
            DateOnly? dateOfBirthMax = null,
            string? latitude = null,
            string? longitude = null,
            CancellationToken cancellationToken = default)
        {

            var query = await GetQueryableAsync();

            query = ApplyFilter(query, filterText, securityNumber, biologicalSex, dateOfBirthMin, dateOfBirthMax, latitude, longitude);

            var ids = query.Select(x => x.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<UserProfile>> GetListAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, securityNumber, biologicalSex, dateOfBirthMin, dateOfBirthMax, latitude, longitude);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserProfileConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? securityNumber = null,
            BiologicalSex? biologicalSex = null,
            DateOnly? dateOfBirthMin = null,
            DateOnly? dateOfBirthMax = null,
            string? latitude = null,
            string? longitude = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, securityNumber, biologicalSex, dateOfBirthMin, dateOfBirthMax, latitude, longitude);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<UserProfile> ApplyFilter(
            IQueryable<UserProfile> query,
            string? filterText = null,
            string? securityNumber = null,
            BiologicalSex? biologicalSex = null,
            DateOnly? dateOfBirthMin = null,
            DateOnly? dateOfBirthMax = null,
            string? latitude = null,
            string? longitude = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.SecurityNumber!.Contains(filterText!) || e.Latitude!.Contains(filterText!) || e.Longitude!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(securityNumber), e => e.SecurityNumber.Contains(securityNumber))
                    .WhereIf(biologicalSex.HasValue, e => e.BiologicalSex == biologicalSex)
                    .WhereIf(dateOfBirthMin.HasValue, e => e.DateOfBirth >= dateOfBirthMin!.Value)
                    .WhereIf(dateOfBirthMax.HasValue, e => e.DateOfBirth <= dateOfBirthMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(latitude), e => e.Latitude.Contains(latitude))
                    .WhereIf(!string.IsNullOrWhiteSpace(longitude), e => e.Longitude.Contains(longitude));
        }
    }
}