using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Json;
using Volo.Abp.Uow;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Microsoft.Extensions.Logging;
using Imaar.Localization;
using System.Net.Http;

namespace Imaar.Authorizations
{
    [RemoteService(false)]
    [AllowAnonymous]

    public class AuthorizationMobileAppService : ApplicationService, IAuthorizationMobileAppService
    {
        private readonly UserManager _userManager;
        private readonly ILogger<AuthorizationMobileAppService> _logger;
        private readonly IConfiguration _configuration;


        public AuthorizationMobileAppService(UserManager userManager,
            IHttpClientFactory httpClientFactory,
            IJsonSerializer jsonSerializer,
            Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor,
            ILogger<AuthorizationMobileAppService> logger,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
            LocalizationResource = typeof(ImaarResource);
        }


        [UnitOfWork(isTransactional: false)]

        public async Task<bool> AuthorizeAsync(string uId, string userName, string email, string password)
        {




            var user = await _userManager.FindUserByClientId(uId);
            if(user != null)
            {
    

            }
         

            if (user == null)
            {
                IdentityUser identityUser = await _userManager.CreateUserAsync(
                                            id: Guid.Parse(uId),
                                            userName: userName,
                                            email: email,
                                            password: password,
                                            name: userName,
                                            surname: userName
                                            );
                if(identityUser != null)
                    return true;
                return false;


            }
            return false;
     

        }

        public async Task<object> GetAbpLogInAsync(TokenRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName) ??
            await _userManager.FindByEmailAsync(request.UserName);

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
               // return Unauthorized();
            }

            var data = await _userManager.GetTokenAsync(user, request.Password);

            if (data.IsError)
                throw new Exception(data.Error);

            var responseResult = new
            {
                Token = new
                {
                    access_token = data.AccessToken,
                    token_type = data.TokenType,
                    expires_in = data.ExpiresIn,
                    refresh_token = data.RefreshToken
                }

            };


            return responseResult;
        }

    }
}
