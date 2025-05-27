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

namespace Imaar.SecondaryAmenities
{
    public class EfCoreSecondaryAmenityRepository : EfCoreSecondaryAmenityRepositoryBase, ISecondaryAmenityRepository
    {
        public EfCoreSecondaryAmenityRepository(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}