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

namespace Imaar.FurnishingLevels
{
    public class EfCoreFurnishingLevelRepository : EfCoreFurnishingLevelRepositoryBase, IFurnishingLevelRepository
    {
        public EfCoreFurnishingLevelRepository(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}