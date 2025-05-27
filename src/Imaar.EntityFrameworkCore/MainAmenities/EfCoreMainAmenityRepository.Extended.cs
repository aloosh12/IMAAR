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

namespace Imaar.MainAmenities
{
    public class EfCoreMainAmenityRepository : EfCoreMainAmenityRepositoryBase, IMainAmenityRepository
    {
        public EfCoreMainAmenityRepository(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}