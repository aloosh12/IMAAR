using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.VerificationCodes;
using Imaar.MobileResponses;

namespace Imaar.Controllers.VerificationCodes
{
    [RemoteService]
    [Area("app")]
    [ControllerName("VerificationCode")]
    [Route("api/mobile/verification-codes")]

    public class VerificationCodeController : VerificationCodeControllerBase
    {
        public VerificationCodeController(IVerificationCodesAppService verificationCodesAppService) : base(verificationCodesAppService)
        {
        }

        [HttpPost("send-secret-key")]
        public virtual Task<MobileResponseDto> SendSecretKey(string email)
        {
            return _verificationCodesAppService.CreateMobileAsync(email);
        }

        [HttpPost("verrify-code")]
        public virtual Task<bool> VerfiyCodeAsync(VerificationCodeMobileDto verificationCodeMobileDto)
        {
            return _verificationCodesAppService.VerifyMobileAsync(verificationCodeMobileDto);
        }
    }
}