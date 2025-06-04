using System;
using System.Threading.Tasks;

namespace Imaar.ImaarServices
{
    public partial interface IImaarServiceRepository
    {
        Task<ImaarServiceWithDetails> GetWithDetailsAsync(Guid id);
    }
}