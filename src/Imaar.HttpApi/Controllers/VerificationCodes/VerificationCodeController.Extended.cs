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
        public virtual Task<MobileResponseDto> SendSecretKey(string phone)
        {
            return _verificationCodesAppService.CreateMobileAsync(phone);
        }

        [HttpPost("verrify-code")]
        public virtual Task<bool> VerfiyCodeAsync(VerificationCodeDto verificationCodeDto)
        {
            return _verificationCodesAppService.VerifyMobileAsync(verificationCodeDto);
        }
    }
}