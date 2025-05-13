using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using Volo.Abp.AspNetCore.Mvc;
using NUglify;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Asp.Versioning;
using Imaar.Authorizations;
using Imaar.MobileResponses;

namespace Imaar.Controllers.Authorizations
{
    [Area("app")]
    [RemoteService]
    [ControllerName("AuthController")]
    [Route("api/mobile/auth")]

    public class AuthController : AbpController, IAuthorizationMobileAppService
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthorizationMobileAppService _authorizeMobileAppService;
        //  private readonly ITokenAcquisition _tokenAcquisition;
        private readonly ILogger<AuthController> _logger;



        public AuthController(
            IConfiguration configuration,
            IAuthorizationMobileAppService authorizeMobileAppService,
                        ILogger<AuthController> logger


            // ITokenAcquisition tokenAcquisition
            )
        {
            _configuration = configuration;
            _authorizeMobileAppService = authorizeMobileAppService;
            _logger = logger;

            // _tokenAcquisition = tokenAcquisition;
        }

        [HttpPost]
        [Route("abp-login-url")]
        public Task<MobileResponseDto> GetAbpLogInAsync(TokenRequest request)
        {
            return _authorizeMobileAppService.GetAbpLogInAsync(request);

        }
    }
}