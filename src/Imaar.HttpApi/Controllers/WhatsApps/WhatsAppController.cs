using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.UserProfiles;
using Volo.Abp.Content;
using Imaar.Shared;
using Imaar.WhatsApps;
using Imaar.MobileResponses;
using Microsoft.AspNetCore.Authorization;

namespace Imaar.Controllers.WhatsApps
{
    [RemoteService]
    [Area("app")]
    [ControllerName("WhatsApp")]
    [Route("api/mobile/whats-app")]

    public class WhatsAppController : AbpController, IWhatsAppService
    {
        protected IWhatsAppService _whatsAppService;

        public WhatsAppController(IWhatsAppService whatsAppService)
        {
            _whatsAppService = whatsAppService;
        }
        [AllowAnonymous]
        [HttpPost("send-secret-key")]
        public virtual Task<MobileResponseDto> SendSecurityCodeAsync(string phone)
        {
            return _whatsAppService.SendSecurityCodeAsync(phone);
            return null;
            // return _userProfilesAppService.VerifySecrityCodeAsync(input);
        }

        [AllowAnonymous]
        [HttpPost("send-message")]
        public virtual Task<MobileResponseDto> SendMessageAsync(WhatsAppMessageDto input)
        {
            return _whatsAppService.SendMessageAsync(input);
            // return null;
            // return _userProfilesAppService.VerifySecrityCodeAsync(input);
        }


    }
}