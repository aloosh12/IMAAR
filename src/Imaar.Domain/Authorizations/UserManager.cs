using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Settings;
using System.Net.Http;
using Volo.Abp.Json;
using Microsoft.Extensions.Configuration;
using IdentityModel.Client;
using Volo.Abp.Uow;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Data;
using Volo.Abp;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;

namespace Imaar.Authorizations
{
    public class UserManager : IDomainService
    {


        private readonly IdentityUserManager _identityUserManager;
        private readonly IPermissionGrantRepository _permissionGrantRepository;
        private readonly IdentityUserStore _identityUserStore;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IPermissionDefinitionManager _permissionDefinitionManager;
        private readonly IPermissionManager _permissionManager;
        private readonly Microsoft.AspNetCore.Identity.SignInManager<Volo.Abp.Identity.IdentityUser> _signInManager;
        private readonly ISettingProvider _settingProvider;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDataFilter _dataFilter;

        public UserManager(
            IdentityUserManager identityUserManager,
            IPermissionGrantRepository permissionGrantRepository,
            IdentityUserStore identityUserStore,
            IIdentityUserRepository identityUserRepository,
            IPermissionDefinitionManager permissionDefinitionManager,
            IPermissionManager permissionManager,
            Microsoft.AspNetCore.Identity.SignInManager<Volo.Abp.Identity.IdentityUser> signInManager,
            ISettingProvider settingProvider,
            IHttpClientFactory httpClientFactory,
            IJsonSerializer jsonSerializer,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IDataFilter dataFilter
            )
        {
            _identityUserManager = identityUserManager;
            _permissionGrantRepository = permissionGrantRepository;
            _identityUserStore = identityUserStore;
            _identityUserRepository = identityUserRepository;
            _permissionDefinitionManager = permissionDefinitionManager;
            _permissionManager = permissionManager;
            _signInManager = signInManager;
            _settingProvider = settingProvider;
            _httpClientFactory = httpClientFactory;
            _jsonSerializer = jsonSerializer;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _dataFilter = dataFilter;
        }

        public Task<Volo.Abp.Identity.IdentityUser> FindByIdAsync(Guid id)
        {
            return _identityUserManager.FindByIdAsync(id.ToString());
        }
        public async Task<Volo.Abp.Identity.IdentityUser> RegisterUserAsync(string firstName, string lastName, string phoneNumber, string email)
        {
            using (_dataFilter.Disable<ISoftDelete>())
            {
                var user = _identityUserManager.Users.Where(u => u.PhoneNumber == phoneNumber || u.Email == email).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsDeleted == true)
                    {
                        //var identityUser = new Volo.Abp.Identity.IdentityUser(user.Id, email.Split("@")[0], user.Email);
                        //identityUser.Name = firstName;
                        //identityUser.Surname = lastName;
                        //identityUser.SetPhoneNumber(phoneNumber, false);
                        //identityUser.IsDeleted = false;
                        //Up
                        //user = email;
                        //user.Surname = userName;
                        //var test = await _userManager.UpdateUserAsync(user);
                    }
                    return null;
                }
                else
                {
                    var identityUser = new Volo.Abp.Identity.IdentityUser(Guid.NewGuid(), email.Split("@")[0], email);
                    identityUser.Name = firstName;
                    identityUser.Surname = lastName;
                    identityUser.SetPhoneNumber(phoneNumber, false);
                    var result = await _identityUserManager.CreateAsync(identityUser);

                    if (result.Succeeded)
                    {
                        return identityUser;
                    }
                    else
                        throw new Volo.Abp.BusinessException(message: result.ToString());
                }
            }
        }


        public Task<Volo.Abp.Identity.IdentityUser> FindByNameAsync(string userName)
        {
            return _identityUserManager.FindByNameAsync(userName);
        }
        public Task<Volo.Abp.Identity.IdentityUser> FindByLoginAsync(string loginProvider, string providerKey)
        {
            return _identityUserManager.FindByLoginAsync(loginProvider, providerKey);
        }
        public Task<Volo.Abp.Identity.IdentityUser> FindByEmailAsync(string email)
        {
            return _identityUserManager.FindByEmailAsync(email);
        }

        public Task<IList<string>> GetUserRoles(IdentityUser user)
        {
            return _identityUserManager.GetRolesAsync(user);
        }

        public Task<IList<Claim>> GetClaimsAsync(IdentityUser user)
        {
            return _identityUserManager.GetClaimsAsync(user);
        }


        public async Task<Microsoft.AspNetCore.Identity.IdentityResult> RemoveUserAsync(string id)
        {
            var user = await _identityUserManager.FindByIdAsync(id);
            return await _identityUserManager.DeleteAsync(user);


        }


        public async Task AddPasswordAsync(Volo.Abp.Identity.IdentityUser user, string password)
        {
            if (user.PasswordHash.IsNullOrEmpty())
            {
                var s = await _identityUserManager.AddPasswordAsync(user, password);
                if (!s.Succeeded)
                    throw new Volo.Abp.UserFriendlyException("AddPasswordAsync " + String.Join(',', s.Errors));
            }
        }

        public async Task<bool> CheckPasswordAsync(Volo.Abp.Identity.IdentityUser user, string password)
        {
            return await _identityUserManager.CheckPasswordAsync(user, password);
        }
        public async Task RemovePasswordAsync(Volo.Abp.Identity.IdentityUser user)
        {
            if (!user.PasswordHash.IsNullOrEmpty())
            {
                var s = await _identityUserManager.RemovePasswordAsync(user);
                if (!s.Succeeded)
                    throw new Volo.Abp.UserFriendlyException("RemovePPasswordAsync " + String.Join(',', s.Errors));
            }

        }

        [UnitOfWork(IsDisabled = true)]
        public async Task<TokenResponse> GetTokenAsync(Volo.Abp.Identity.IdentityUser user, string password)
        {

            var httpClient = _httpClientFactory.CreateClient();

            var tokenResponse = await httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = $"{_configuration["AuthServer:Authority"]}/connect/token",

                ClientId = _configuration["AuthServer:SwaggerClientId"],
               // ClientSecret = _configuration["AuthServer:SwaggerClientSecret"],
                Scope = _configuration["AuthServer:ApiKeyScope"],
                UserName = user.UserName,
                Password = password

            });
            return tokenResponse;
        }

        //public async Task<TokenResponse> RefreshTokenAsync(string token)
        //{

        //    //https://docs.identityserver.io/en/latest/quickstarts/1_client_credentials.html


        //    var httpClient = _httpClientFactory.CreateClient();

        //    var tokenResponse = await httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
        //    {
        //        Address = $"{_configuration["AuthServer:Authority"]}/connect/token",
        //        ClientId = _configuration["AuthServer:ApiKeyClientId"],
        //        ClientSecret = _configuration["AuthServer:ApiKeyClientSecret"],

        //        RefreshToken = token
        //    });



        //    return tokenResponse;
        //}

        public async Task<Volo.Abp.Identity.IdentityUser> FindUserByClientId(string clientId)
        {
            var user = await _identityUserManager.FindByIdAsync(clientId);
           
            return user;
        }

        public async Task SetPermissionForUserAsync(Guid userId, string name, bool isGranted)
        {
            await _permissionManager.SetForUserAsync(userId, name, isGranted);
        }

        public async Task<bool> AddLoginAsync(Volo.Abp.Identity.IdentityUser user, Microsoft.AspNetCore.Identity.UserLoginInfo login)
        {
            var logins = (await _identityUserManager.GetLoginsAsync(user))
                .Where(t => t.LoginProvider == login.LoginProvider && t.ProviderKey == login.ProviderKey && t.ProviderDisplayName == login.ProviderDisplayName);
            if (logins is not null && logins.Any())
                return true;
            var result = await _identityUserManager.AddLoginAsync(user, login);
            return result.Succeeded;
        }

        public async Task<Volo.Abp.Identity.IdentityUser> CreateUserAsync(Guid id, string userName, string email, string password = null, string name = null, string surname = null)
        {
            var identityUser = new Volo.Abp.Identity.IdentityUser(id, userName, email);
            identityUser.Name = name;
            identityUser.Surname = surname;
            var result = await _identityUserManager.CreateAsync(identityUser, password, false);

            if (result.Succeeded)
            {
                //if (!password.IsNullOrEmpty())
                //    try
                //    {
                //        await AddPasswordAsync(identityUser, password);
                //    }
                //    catch(Exception exp)
                //    {
                //        int u = 0;
                //    }
                //else
                //    await AddPasswordAsync(identityUser, GetPassword(identityUser));
                return identityUser;
            }
            else
                throw new Volo.Abp.BusinessException(message: result.ToString());


        }
        public async Task<Volo.Abp.Identity.IdentityUser> UpdateUserAsync(Volo.Abp.Identity.IdentityUser user)
        {

            var result = await _identityUserManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Volo.Abp.BusinessException(message: result.ToString());

            return user;
        }

        public async Task AddUserToRoleAsync(Volo.Abp.Identity.IdentityUser user, string roleName)
        {
            var result = await _identityUserManager.AddToRoleAsync(user, roleName);

            if (!result.Succeeded)
                throw new Volo.Abp.BusinessException(result.ToString());
        }

        public async Task RemoveUserFromRoleAsync(Volo.Abp.Identity.IdentityUser user, string roleName)
        {
            var result = await _identityUserManager.RemoveFromRoleAsync(user, roleName);

            if (!result.Succeeded)
                throw new Volo.Abp.BusinessException(result.ToString());
        }


        public async Task<List<Volo.Abp.Identity.IdentityUser>> GetListUserByPermissionsAsync(List<string> permisssions)
        {

            var directPermissions = (await _permissionGrantRepository.GetListAsync()).Where(t => t.ProviderName == "U" && permisssions.Contains(t.Name));
            var rolesPermissions = (await _permissionGrantRepository.GetListAsync()).Where(t => t.ProviderName == "R" && permisssions.Contains(t.Name));

            var users = new List<Volo.Abp.Identity.IdentityUser>();

            foreach (var role in rolesPermissions)
            {
                var usersinrole = await _identityUserManager.GetUsersInRoleAsync(role.ProviderKey);
                users.AddIfNotContains(usersinrole);
            }
            var usersIds = directPermissions.Select(t => t.ProviderKey).ToList().ConvertAll(Guid.Parse);
            if (usersIds != null && usersIds.Count > 0)
            {
                //var asd = await _identityUserRepository.GetListAsync();


                var usersbyIds = (await _identityUserRepository.GetListAsync()).Where(t => usersIds.Contains(t.Id)).ToList();
                users.AddIfNotContains(usersbyIds);
            }

            return users;
        }

        //ToDo Ali Moved To Net7
        public IReadOnlyList<PermissionDefinition> GetListPermissionByGroupNameAsync(string groupName)
        {
            var group = _permissionDefinitionManager.GetGroupsAsync().Result.SingleOrDefault(t => t.Name == groupName);
            return group.Permissions;
        }

        public async Task<List<string>> GetPermissionListForUserAsync(Guid userId)
        {
            var allUserPermissions = new List<PermissionWithGrantedProviders>();
            var user = await _identityUserManager.FindByIdAsync(userId.ToString());
            var userPermissions = await _permissionManager.GetAllForUserAsync(userId);
            allUserPermissions.AddRange(userPermissions);

            var userRoles = await _identityUserManager.GetRolesAsync(user);
            
            foreach (var role in userRoles)
            {
                allUserPermissions.AddRange(await _permissionManager.GetAllForRoleAsync(role));
            }

            var grantedPermissions = new List<string>();
            allUserPermissions.ForEach(t =>
            {
                if (t.IsGranted)
                    grantedPermissions.AddIfNotContains(t.Name);
            });

            return grantedPermissions;
        }

        public async Task<bool> IsPermissionGrantedAsync(Guid userId, string permission)
        {
            return (await GetPermissionListForUserAsync(userId)).Any(t => t == permission);
        }

        public async Task Logout()
        {

            await _signInManager.SignOutAsync();

        }



        public Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent = false)
        {
            return _signInManager.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent);
        }

        public async Task<int> getCountUser (string startDate ,string endDate)
        {


            var res =  await _identityUserRepository.GetListAsync();
            if(startDate != null && endDate != null)
                 res =  res.Where(t => t.CreationTime >= DateTime.Parse(startDate)  &&   t.CreationTime <= DateTime.Parse(endDate)).ToList();
            else if (startDate != null && endDate == null)
            {
                res = res.Where(t => t.CreationTime >= DateTime.Parse(startDate)).ToList();

            }
            else if (startDate == null && endDate != null)
            {
                res = res.Where(t => t.CreationTime <= DateTime.Parse(endDate)).ToList();

            }
            return res.Count();
        }

        public async Task<IdentityResult> ResetPasswordForEmailAsync(string email, string token, string newPassword)
        {
            var user = await _identityUserManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new UserFriendlyException("User not found with this email address.");
            }

            var result = await _identityUserManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                throw new UserFriendlyException("Failed to reset password: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return result;
        }

        public async Task<string> GeneratePasswordResetTokenForEmailAsync(string email)
        {
            var user = await _identityUserManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new UserFriendlyException("User not found with this email address.");
            }

            return await _identityUserManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordWithoutTokenAsync(string email, string newPassword)
        {
            var user = await _identityUserManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new UserFriendlyException("User not found with this email address.");
            }

            // Remove existing password
            await RemovePasswordAsync(user);

            // Add new password
            var result = await _identityUserManager.AddPasswordAsync(user, newPassword);
            if (!result.Succeeded)
            {
                throw new UserFriendlyException("Failed to reset password: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return result;
        }

        public async Task<IdentityResult> ChangePasswordAsync(string email, string oldPassword, string newPassword)
        {
            var user = await _identityUserManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new UserFriendlyException("User not found with this email address.");
            }

            // Remove existing password
            await RemovePasswordAsync(user);

            // Add new password
            var result = await _identityUserManager.AddPasswordAsync(user, newPassword);
            if (!result.Succeeded)
            {
                throw new UserFriendlyException("Failed to reset password: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return result;
        }
    }
}
