using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.VerificationCodes
{
    public abstract class VerificationCodeManagerBase : DomainService
    {
        protected IVerificationCodeRepository _verificationCodeRepository;

        public VerificationCodeManagerBase(IVerificationCodeRepository verificationCodeRepository)
        {
            _verificationCodeRepository = verificationCodeRepository;
        }

        public virtual async Task<VerificationCode> CreateAsync(
        string phoneNumber, int securityCode, bool isFinish)
        {
            Check.NotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));

            var verificationCode = new VerificationCode(
             GuidGenerator.Create(),
             phoneNumber, securityCode, isFinish
             );

            return await _verificationCodeRepository.InsertAsync(verificationCode);
        }

        public virtual async Task<VerificationCode> UpdateAsync(
            Guid id,
            string phoneNumber, int securityCode, bool isFinish, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));

            var verificationCode = await _verificationCodeRepository.GetAsync(id);

            verificationCode.PhoneNumber = phoneNumber;
            verificationCode.SecurityCode = securityCode;
            verificationCode.IsFinish = isFinish;

            verificationCode.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _verificationCodeRepository.UpdateAsync(verificationCode);
        }

    }
}