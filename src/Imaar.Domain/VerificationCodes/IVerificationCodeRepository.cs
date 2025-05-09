using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Imaar.VerificationCodes
{
    public partial interface IVerificationCodeRepository : IRepository<VerificationCode, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? phoneNumber = null,
            int? securityCodeMin = null,
            int? securityCodeMax = null,
            bool? isFinish = null,
            CancellationToken cancellationToken = default);
        Task<List<VerificationCode>> GetListAsync(
                    string? filterText = null,
                    string? phoneNumber = null,
                    int? securityCodeMin = null,
                    int? securityCodeMax = null,
                    bool? isFinish = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? phoneNumber = null,
            int? securityCodeMin = null,
            int? securityCodeMax = null,
            bool? isFinish = null,
            CancellationToken cancellationToken = default);
    }
}