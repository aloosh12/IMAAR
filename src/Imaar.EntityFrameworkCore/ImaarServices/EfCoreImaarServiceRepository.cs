using Imaar.UserProfiles;
using Imaar.ServiceTypes;
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

namespace Imaar.ImaarServices
{
    public abstract class EfCoreImaarServiceRepositoryBase : EfCoreRepository<ImaarDbContext, ImaarService, Guid>
    {
        public EfCoreImaarServiceRepositoryBase(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
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
            Guid? serviceTypeId = null,
            Guid? userProfileId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, title, description, serviceLocation, serviceNumber, dateOfPublishMin, dateOfPublishMax, priceMin, priceMax, latitude, longitude, serviceTypeId, userProfileId);

            var ids = query.Select(x => x.ImaarService.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<ImaarServiceWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(imaarService => new ImaarServiceWithNavigationProperties
                {
                    ImaarService = imaarService,
                    ServiceType = dbContext.Set<ServiceType>().FirstOrDefault(c => c.Id == imaarService.ServiceTypeId),
                    UserProfile = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == imaarService.UserProfileId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<ImaarServiceWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
            Guid? serviceTypeId = null,
            Guid? userProfileId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, title, description, serviceLocation, serviceNumber, dateOfPublishMin, dateOfPublishMax, priceMin, priceMax, latitude, longitude, serviceTypeId, userProfileId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ImaarServiceConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<ImaarServiceWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from imaarService in (await GetDbSetAsync())
                   join serviceType in (await GetDbContextAsync()).Set<ServiceType>() on imaarService.ServiceTypeId equals serviceType.Id into serviceTypes
                   from serviceType in serviceTypes.DefaultIfEmpty()
                   join userProfile in (await GetDbContextAsync()).Set<UserProfile>() on imaarService.UserProfileId equals userProfile.Id into userProfiles
                   from userProfile in userProfiles.DefaultIfEmpty()
                   select new ImaarServiceWithNavigationProperties
                   {
                       ImaarService = imaarService,
                       ServiceType = serviceType,
                       UserProfile = userProfile
                   };
        }

        protected virtual IQueryable<ImaarServiceWithNavigationProperties> ApplyFilter(
            IQueryable<ImaarServiceWithNavigationProperties> query,
            string? filterText,
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
            Guid? serviceTypeId = null,
            Guid? userProfileId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.ImaarService.Title!.Contains(filterText!) || e.ImaarService.Description!.Contains(filterText!) || e.ImaarService.ServiceLocation!.Contains(filterText!) || e.ImaarService.ServiceNumber!.Contains(filterText!) || e.ImaarService.Latitude!.Contains(filterText!) || e.ImaarService.Longitude!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(title), e => e.ImaarService.Title.Contains(title))
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.ImaarService.Description.Contains(description))
                    .WhereIf(!string.IsNullOrWhiteSpace(serviceLocation), e => e.ImaarService.ServiceLocation.Contains(serviceLocation))
                    .WhereIf(!string.IsNullOrWhiteSpace(serviceNumber), e => e.ImaarService.ServiceNumber.Contains(serviceNumber))
                    .WhereIf(dateOfPublishMin.HasValue, e => e.ImaarService.DateOfPublish >= dateOfPublishMin!.Value)
                    .WhereIf(dateOfPublishMax.HasValue, e => e.ImaarService.DateOfPublish <= dateOfPublishMax!.Value)
                    .WhereIf(priceMin.HasValue, e => e.ImaarService.Price >= priceMin!.Value)
                    .WhereIf(priceMax.HasValue, e => e.ImaarService.Price <= priceMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(latitude), e => e.ImaarService.Latitude.Contains(latitude))
                    .WhereIf(!string.IsNullOrWhiteSpace(longitude), e => e.ImaarService.Longitude.Contains(longitude))
                    .WhereIf(serviceTypeId != null && serviceTypeId != Guid.Empty, e => e.ServiceType != null && e.ServiceType.Id == serviceTypeId)
                    .WhereIf(userProfileId != null && userProfileId != Guid.Empty, e => e.UserProfile != null && e.UserProfile.Id == userProfileId);
        }

        public virtual async Task<List<ImaarService>> GetListAsync(
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
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, title, description, serviceLocation, serviceNumber, dateOfPublishMin, dateOfPublishMax, priceMin, priceMax, latitude, longitude);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ImaarServiceConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
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
            Guid? serviceTypeId = null,
            Guid? userProfileId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, title, description, serviceLocation, serviceNumber, dateOfPublishMin, dateOfPublishMax, priceMin, priceMax, latitude, longitude, serviceTypeId, userProfileId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<ImaarService> ApplyFilter(
            IQueryable<ImaarService> query,
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
            string? longitude = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Title!.Contains(filterText!) || e.Description!.Contains(filterText!) || e.ServiceLocation!.Contains(filterText!) || e.ServiceNumber!.Contains(filterText!) || e.Latitude!.Contains(filterText!) || e.Longitude!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(title), e => e.Title.Contains(title))
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description))
                    .WhereIf(!string.IsNullOrWhiteSpace(serviceLocation), e => e.ServiceLocation.Contains(serviceLocation))
                    .WhereIf(!string.IsNullOrWhiteSpace(serviceNumber), e => e.ServiceNumber.Contains(serviceNumber))
                    .WhereIf(dateOfPublishMin.HasValue, e => e.DateOfPublish >= dateOfPublishMin!.Value)
                    .WhereIf(dateOfPublishMax.HasValue, e => e.DateOfPublish <= dateOfPublishMax!.Value)
                    .WhereIf(priceMin.HasValue, e => e.Price >= priceMin!.Value)
                    .WhereIf(priceMax.HasValue, e => e.Price <= priceMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(latitude), e => e.Latitude.Contains(latitude))
                    .WhereIf(!string.IsNullOrWhiteSpace(longitude), e => e.Longitude.Contains(longitude));
        }
    }
}