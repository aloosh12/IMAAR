using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.UserProfiles;
using Imaar.MobileResponses;
using Microsoft.AspNetCore.Authorization;
using Imaar.WhatsApps;

namespace Imaar.Controllers.UserProfiles
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserProfile")]
    [Route("api/mobile/user-profiles")]

    public class UserProfileController : UserProfileControllerBase, IUserProfilesAppService
    {
        public UserProfileController(IUserProfilesAppService userProfilesAppService) : base(userProfilesAppService)
        {
        }

        [AllowAnonymous]
        [HttpPost("register-user")]
        public virtual Task<MobileResponseDto> RegisterAsync(RegisterCreateDto input)
        {
            return _userProfilesAppService.RegisterAsync(input);
        }

        [AllowAnonymous]
        [HttpPost("verify-secret-key")]
        public virtual Task<MobileResponseDto> VerifySecurityCodeAsync(SecurityNumberCreateDto input)
        {
            return null;
            //return _userProfilesAppService.VerifySecrityCodeAsync(input);
        }




        //[AllowAnonymous]
        //[HttpPost("complete-registration")]
        //public virtual Task<MobileResponseDto> CompleteRegisterStep1Async(RegisterCreateDt0 input)
        //{
        //    return _userProfilesAppService.RegisterStep1Async(input);
        //}
    }
}