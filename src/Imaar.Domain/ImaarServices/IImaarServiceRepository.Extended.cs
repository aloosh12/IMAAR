using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Imaar.ImaarServices
{
    public partial interface IImaarServiceRepository
    {
        Task<ImaarServiceWithDetails> GetWithDetailsAsync(Guid id);
    }
}