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
using Imaar.UserWorksExhibitions;
using Volo.Abp.Identity;
using Volo.Abp.Domain.Repositories;
using Imaar.Tickets;

namespace Imaar.UserProfiles
{
    public class EfCoreUserProfileRepository : EfCoreUserProfileRepositoryBase, IUserProfileRepository
    {
        private readonly IIdentityUserRepository _identityUserRepository;
        public EfCoreUserProfileRepository(IDbContextProvider<ImaarDbContext> dbContextProvider, IIdentityUserRepository identityUserRepository)
            : base(dbContextProvider)
        {
            _identityUserRepository = identityUserRepository;
        }

        public virtual async Task<UserProfileWithDetails> GetWithDetailsAsync(Guid id)
        {
            var dbContext = await GetDbContextAsync();
            var identityuser = dbContext.Set<IdentityUser>().FirstOrDefault(c => c.Id == id);
            var roles = await _identityUserRepository.GetRolesAsync(identityuser.Id);
            var rolId = roles.Any(r => r.Id == Guid.Parse("3454fc01-7d85-48cf-9d7d-3a19d0565a75")) ? "2" : (roles.Any(r => r.Id == Guid.Parse("84840acb-9a32-4fc8-7b98-3a19d056874e")) ? "1" : "3");

            // Calculate averages from UserEvalauation
            var evals = dbContext.Set<Imaar.UserEvalauations.UserEvalauation>()
                .Where(e => e.EvaluatedPersonId == id);

            double avgSpeedOfCompletion = await evals.AnyAsync() ? await evals.AverageAsync(e => (double?)e.SpeedOfCompletion) ?? 0 : 0;
            double avgDealing = await evals.AnyAsync() ? await evals.AverageAsync(e => (double?)e.Dealing) ?? 0 : 0;
            double avgCleanliness = await evals.AnyAsync() ? await evals.AverageAsync(e => (double?)e.Cleanliness) ?? 0 : 0;
            double avgPerfection = await evals.AnyAsync() ? await evals.AverageAsync(e => (double?)e.Perfection) ?? 0 : 0;
            double avgPrice = await evals.AnyAsync() ? await evals.AverageAsync(e => (double?)e.Price) ?? 0 : 0;

            return (await GetDbSetAsync())
                   .Where(userProfile => userProfile.Id == id)
                .Select(userProfile => new UserProfileWithDetails
                {
                    UserProfile = userProfile,
                    UserWorksExhibitionList = dbContext.Set<UserWorksExhibition>().Where(u => u.UserProfileId == id).OrderBy(u => u.Order).ToList(),
                    IdentityUser = identityuser,
                    Role = rolId,
                    SpeedOfCompletion = avgSpeedOfCompletion,
                    Dealing = avgDealing,
                    Cleanliness = avgCleanliness,
                    Perfection = avgPerfection,
                    Price = avgPrice
                }).FirstOrDefault();
        }
    }
}