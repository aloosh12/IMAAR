using Imaar.UserProfiles;
using Imaar.EntityFrameworkCore;
using Imaar.UserProfiles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

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
            string? firstName = null,
            string? lastName = null,
            string? phoneNumber = null,
            string? email = null,
            CancellationToken cancellationToken = default)
        {

            var query = await GetQueryableAsync();

            query = ApplyFilter(query, filterText, securityNumber, biologicalSex, dateOfBirthMin, dateOfBirthMax, latitude, longitude, firstName, lastName, phoneNumber, email);

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
            string? firstName = null,
            string? lastName = null,
            string? phoneNumber = null,
            string? email = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, securityNumber, biologicalSex, dateOfBirthMin, dateOfBirthMax, latitude, longitude, firstName, lastName, phoneNumber, email);
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
            string? firstName = null,
            string? lastName = null,
            string? phoneNumber = null,
            string? email = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, securityNumber, biologicalSex, dateOfBirthMin, dateOfBirthMax, latitude, longitude, firstName, lastName, phoneNumber, email);
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
            string? longitude = null,
            string? firstName = null,
            string? lastName = null,
            string? phoneNumber = null,
            string? email = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.SecurityNumber!.Contains(filterText!) || e.Latitude!.Contains(filterText!) || e.Longitude!.Contains(filterText!) || e.FirstName!.Contains(filterText!) || e.LastName!.Contains(filterText!) || e.PhoneNumber!.Contains(filterText!) || e.Email!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(securityNumber), e => e.SecurityNumber.Contains(securityNumber))
                    .WhereIf(biologicalSex.HasValue, e => e.BiologicalSex == biologicalSex)
                    .WhereIf(dateOfBirthMin.HasValue, e => e.DateOfBirth >= dateOfBirthMin!.Value)
                    .WhereIf(dateOfBirthMax.HasValue, e => e.DateOfBirth <= dateOfBirthMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(latitude), e => e.Latitude.Contains(latitude))
                    .WhereIf(!string.IsNullOrWhiteSpace(longitude), e => e.Longitude.Contains(longitude))
                    .WhereIf(!string.IsNullOrWhiteSpace(firstName), e => e.FirstName.Contains(firstName))
                    .WhereIf(!string.IsNullOrWhiteSpace(lastName), e => e.LastName.Contains(lastName))
                    .WhereIf(!string.IsNullOrWhiteSpace(phoneNumber), e => e.PhoneNumber.Contains(phoneNumber))
                    .WhereIf(!string.IsNullOrWhiteSpace(email), e => e.Email.Contains(email));
        }
    }
}