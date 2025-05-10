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

namespace Imaar.TicketTypes
{
    public class EfCoreTicketTypeRepository : EfCoreTicketTypeRepositoryBase, ITicketTypeRepository
    {
        public EfCoreTicketTypeRepository(IDbContextProvider<ImaarDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}