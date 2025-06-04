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
using Imaar.ServiceTypes;
using Imaar.UserProfiles;
using Imaar.Medias;

namespace Imaar.ImaarServices
{
    public class EfCoreImaarServiceRepository : EfCoreImaarServiceRepositoryBase, IImaarServiceRepository
    {
        public EfCoreImaarServiceRepository(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<ImaarServiceWithDetails> GetWithDetailsAsync(Guid id)
        {
            var dbContext = await GetDbContextAsync();

            return (from imaarService in (await GetDbSetAsync())
                    where imaarService.Id == id
                    join serviceType in (await GetDbContextAsync()).Set<ServiceType>() on imaarService.ServiceTypeId equals serviceType.Id into serviceTypes
                    from serviceType in serviceTypes.DefaultIfEmpty()
                    join userProfile in (await GetDbContextAsync()).Set<UserProfile>() on imaarService.UserProfileId equals userProfile.Id into userProfiles
                    from userProfile in userProfiles.DefaultIfEmpty()
                    select new ImaarServiceWithDetails
                    {
                        ImaarService = imaarService,
                        ServiceType = serviceType,
                        UserProfile = userProfile,  // Changed from userProfiles to userProfile
                                                    //  Medias = (await GetDbContextAsync()).Set<Media>().Where(m => m.ImaarServiceId == id).ToList()
                    }).FirstOrDefault();
        }
    }
}