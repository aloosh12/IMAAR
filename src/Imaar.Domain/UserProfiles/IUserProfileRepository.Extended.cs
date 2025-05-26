using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Imaar.UserProfiles
{
    public partial interface IUserProfileRepository
    {
        public Task<UserProfileWithDetails> GetWithDetailsAsync(Guid id);
    }
}