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

namespace Imaar.UserWorksExhibitions
{
    public class EfCoreUserWorksExhibitionRepository : EfCoreUserWorksExhibitionRepositoryBase, IUserWorksExhibitionRepository
    {
        public EfCoreUserWorksExhibitionRepository(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}