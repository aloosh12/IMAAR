﻿using Microsoft.AspNetCore.Authorization;
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
using static Volo.Abp.Identity.Settings.IdentitySettingNames;
using static Volo.Abp.UI.Navigation.DefaultMenuNames.Application;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Linq;
using Imaar.MobileResponses;

namespace Imaar.Authorizations
{
    [RemoteService(false)]
    [AllowAnonymous]

    public class AuthorizationMobileAppService : ApplicationService, IAuthorizationMobileAppService
    {
        private readonly UserManager _userManager;
        private readonly ILogger<AuthorizationMobileAppService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IIdentityUserRepository _identityUserRepository;


        public AuthorizationMobileAppService(UserManager userManager,
            IHttpClientFactory httpClientFactory,
            IJsonSerializer jsonSerializer,
            Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor,
            ILogger<AuthorizationMobileAppService> logger,
            IConfiguration configuration
,
            IIdentityUserRepository identityUserRepository)
        {
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
            LocalizationResource = typeof(ImaarResource);
            _identityUserRepository = identityUserRepository;
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

        public async Task<MobileResponseDto> GetAbpLogInAsync(TokenRequest request)
        {
            //if(request.UserName == "admin" && request.Password == "As12345678!")
            //{
            //    var responseResult = new
            //    {
            //        Token = new
            //        {
            //            access_token = "eyJhbGciOiJ11SUzI1NiIsImtpZCI6IjYzMjRFRDVFMEZGM0MwOEM3NUFFRUU0QkQ5MDUyNzdDNDMzQjNFMzIiLCJ4NXQiOiJZeVR0WGdfendJeDFydTVMMlFVbmZFTTdQakkiLCJ0eXAiOiJhdCtqd3QifQ.eyJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo0NDM1Mi8iLCJleHAiOjE3NDYxNDg3NzQsImlhdCI6MTc0NjE0NTE3NCwiYXVkIjoiSW1hYXIiLCJzY29wZSI6IkltYWFyIiwianRpIjoiMTMwYzU3OGItZTFjZS00YjlhLThjZDAtMmRmZTJhZTY3YWRiIiwic3ViIjoiNjc0MTZkMjYtYmJiYi0wMjViLTkxY2ItM2ExOWEwODZjNmRkIiwicHJlZmVycmVkX3VzZXJuYW1lIjoiYWRtaW4iLCJlbWFpbCI6ImFkbWluQGFicC5pbyIsInJvbGUiOiJhZG1pbiIsImdpdmVuX25hbWUiOiJhZG1pbiIsInBob25lX251bWJlcl92ZXJpZmllZCI6IkZhbHNlIiwiZW1haWxfdmVyaWZpZWQiOiJGYWxzZSIsInVuaXF1ZV9uYW1lIjoiYWRtaW4iLCJvaV9wcnN0IjoiSW1hYXJfU3dhZ2dlciIsImNsaWVudF9pZCI6IkltYWFyX1N3YWdnZXIiLCJvaV90a25faWQiOiJkNGU0ODZmOS1iN2M2LWZhMTUtOGRkZC0zYTE5YTA4OWJjYWYifQ.VrXv4uUSxkXc93sf3dG78KEk9R85OYsCJWUMomdTtzBZnXp6ooswtikVzAkZu1_dVd-bGnRGYmVO2sd4FPD_7jKrMlNCs6CNgJ9Yb-UjcPYChODyZ0TOa6NKsXiLw95QzkrB0kcrl_BVDkykdZfthbK2vY25u93Tlzw0SjJ2UrztrdZoqHJ-0DMSu6nefFZO-qzlP5omf4WsZikz3FTFpM71KDXwQjQTbNMgLNKu1LGaQikNI-9bx9Yznx-EhvoMM2HHxGuyOvCP0KKj0PQK9bRDHVl0wSFA13ceevfdIqA6aJsW_-hmYGgcOstfWb9Ct3m3CQvJhw1dbYycdcU-4g",
            //            token_type = "TokenType",
            //            expires_in = "3600",
            //            refresh_token = ""
            //        }

            //    };

            //    return responseResult;
            //}

            var users = await _identityUserRepository.GetListAsync();
            var user = users.FirstOrDefault(u => u.PhoneNumber == request.PhoneNumber);
            MobileResponseDto mobileResponseDto = new MobileResponseDto();
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                mobileResponseDto.Code = 401;
                mobileResponseDto.Message = "Wrong phone or password";
                mobileResponseDto.Data = null;
                return mobileResponseDto;
                // return Unauthorized();
            }
            var data = await _userManager.GetTokenAsync(user, request.Password);

            if (data.IsError)
                throw new Exception(data.Error);

            //var responseResult = new
            //{
            //    Token = new
            //    {
            //        access_token = data.AccessToken,
            //        token_type = data.TokenType,
            //        expires_in = data.ExpiresIn,
            //        refresh_token = data.RefreshToken
            //    }

            //};
            TokenResponse tokenResponse = new TokenResponse();
            tokenResponse.AccessToken = data.AccessToken;
            tokenResponse.TokenType = data.TokenType;
            tokenResponse.ExpiresIn = data.ExpiresIn.ToString();
            tokenResponse.Email = user.Email;
            tokenResponse.Phone = user.PhoneNumber;
            tokenResponse.FirstName = user.Name; ;
            tokenResponse.LastName = user.Surname;
            tokenResponse.UserId = user.Id.ToString();
            Guid serviceProvided = Guid.Parse("");
            Guid normalUser = Guid.Parse("");
            Guid admin = Guid.Parse("");
            if (user.Roles.Any(r => r.RoleId == normalUser))
                tokenResponse.RoleId = 1;
            else
            {
                if (user.Roles.Any(r => r.RoleId == serviceProvided))
                    tokenResponse.RoleId = 2;
                else
                {
                    if (user.Roles.Any(r => r.RoleId == admin))
                        tokenResponse.RoleId = 3;

                }
            }
            // tokenResponse.refresh_token = data.RefreshToken;

            mobileResponseDto.Code = 200;
            mobileResponseDto.Message = "SUCCESS";
            mobileResponseDto.Data = tokenResponse;
            return (mobileResponseDto);

            return null;
        }
            //public async Task<object> GetAbpLogInAsync(TokenRequest request)
            //{
            //    var user = await _userManager.FindByNameAsync(request.UserName) ??
            //    await _userManager.FindByEmailAsync(request.UserName);

            //    if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            //    {
            //       // return Unauthorized();
            //    }

            //    var data = await _userManager.GetTokenAsync(user, request.Password);

            //    if (data.IsError)
            //        throw new Exception(data.Error);

            //    var responseResult = new
            //    {
            //        Token = new
            //        {
            //            access_token = data.AccessToken,
            //            token_type = data.TokenType,
            //            expires_in = data.ExpiresIn,
            //            refresh_token = data.RefreshToken
            //        }

            //    };


            //    return responseResult;
            //}

        }
}
