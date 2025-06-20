using Imaar.Authorizations;
using Imaar.MobileResponses;
using Imaar.Permissions;
using Imaar.Shared;
using Imaar.StoryLovers;
using Imaar.UserFollows;
using Imaar.UserProfiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Imaar.UserProfiles
{
    public class UserProfilesAppService : UserProfilesAppServiceBase, IUserProfilesAppService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IUserFollowsAppService _userFollowsAppService;
        private readonly IUserFollowRepository _userFollowRepository;
        
        //<suite-custom-code-autogenerated>
        public UserProfilesAppService(
            IUserProfileRepository userProfileRepository, 
            UserProfileManager userProfileManager, 
            UserManager userManager,  
            IDistributedCache<UserProfileDownloadTokenCacheItem, string> downloadTokenCache,
            ICurrentUser currentUser,
            IUserFollowsAppService userFollowsAppService, IUserFollowRepository userFollowRepository)
            : base(userProfileRepository, userProfileManager, userManager, downloadTokenCache)
        {
            _currentUser = currentUser;
            _userFollowsAppService = userFollowsAppService;
            _userFollowRepository = userFollowRepository;
        }
        //</suite-custom-code-autogenerated>

        //Write your custom code...
        [AllowAnonymous]
        public virtual async Task<MobileResponseDto> RegisterAsync(RegisterCreateDto input)
        {
            var user = await _userProfileManager.CreatWithDetialsAsync(input.FirstName, input.LastName, input.PhoneNumber, input.Email, input.Password, input.SecurityCode, input.BiologicalSex, input.DateOfBirth, input.Latitude, input.Longitude, input.ProfilePhoto, input.RoleId);
            return ObjectMapper.Map<MobileResponse, MobileResponseDto>(user);
        }

        public virtual async Task<MobileResponseDto> UpdateUserDetailsAsync(UserUpdateDto input)
        {
            var user = await _userProfileManager.UpdateWithDetailsAsync(input.UserId, input.FirstName, input.LastName, input.PhoneNumber, input.Email, input.BiologicalSex, input.ProfilePhoto);
            return ObjectMapper.Map<MobileResponse, MobileResponseDto>(user);
        }

        [AllowAnonymous]
        public virtual async Task<MobileResponseDto> VerifyEmailExistsAsync(VerifyEmailExistsDto input)
        {
            var mobileResponse = new MobileResponse();
            try
            {
                var user = await _userManager.FindByEmailAsync(input.Email);
                if (user == null)
                {
                    mobileResponse.Code = 404;
                    mobileResponse.Message = "Email not found";
                    mobileResponse.Data = null;
                }
                else
                {
                    mobileResponse.Code = 200;
                    mobileResponse.Message = "Email exists";
                    mobileResponse.Data = new { Email = input.Email };
                }
            }
            catch (Exception ex)
            {
                mobileResponse.Code = 500;
                mobileResponse.Message = ex.Message;
                mobileResponse.Data = null;
            }
            return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
        }

        [AllowAnonymous]
        public virtual async Task<MobileResponseDto> VerifySecurityCodeAsync(SecurityNumberCreateDto input)
        {
            return null;
        }
        
        public virtual async Task<bool> CheckFollowStatusAsync(Guid id)
        {
            var currentUserId = _currentUser.Id;
            if (currentUserId == null)
            {
                return false;
            }
            var input = new GetUserFollowsInput
            {
                FollowerUserId = currentUserId,
                FollowingUserId = id,
                MaxResultCount = 1
            };

            var totalCount = await _userFollowRepository.GetCountAsync(input.FilterText, input.FollowerUserId, input.FollowingUserId);
            return totalCount > 0;
        }
        
        public virtual async Task<long> GetFollowCountAsync(Guid id)
        {
            // Get count of followers for this user profile
            var input = new GetUserFollowsInput
            {
                FollowingUserId = id,  // Count users who follow this profile
                MaxResultCount = 1  // We only need count, not the actual items
            };

            var totalCount = await _userFollowRepository.GetCountAsync(input.FilterText, input.FollowerUserId, input.FollowingUserId);
            return totalCount;
        }

        public virtual async Task<bool> UnfollowUserAsync(Guid id)
        {


            var currentUserId = _currentUser.Id;
            if (currentUserId == null)
            {
                throw new UserFriendlyException("Current user not exist");
            }
            var userFollow = await _userFollowRepository.FirstOrDefaultAsync(x => x.FollowerUserId == currentUserId && x.FollowingUserId == id); ;
            if (userFollow == null)
            {
                throw new UserFriendlyException("User Follow not exist");
            }
            await _userFollowsAppService.DeleteAsync(userFollow.Id);
            return true;
        }

        

        //[AllowAnonymous]
        //public virtual async Task<MobileResponseDto> RequestPasswordResetAsync(PasswordResetRequestDto input)
        //{
        //    var user = await _userManager.FindByEmailAsync(input.Email);
        //    if (user == null)
        //    {
        //        throw new UserFriendlyException("User not found with this email address.");
        //    }

        //    // Generate a random security code
        //    Random random = new Random();
        //    string securityCode = random.Next(1000, 10000).ToString();

        //    // Store the security code in cache with expiration
        //    var cacheKey = $"password_reset_{user.Id}";
        //    await _downloadTokenCache.SetAsync(
        //        cacheKey,
        //        new UserProfileDownloadTokenCacheItem { SecurityCode = securityCode },
        //        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) }
        //    );

        //    // TODO: Send security code via email or SMS
        //    // For now, we'll just return it in the response
        //    var response = new MobileResponse
        //    {
        //        Code = 200,
        //        Message = "Security code sent successfully",
        //        Data = new { SecurityCode = securityCode }
        //    };

        //    return ObjectMapper.Map<MobileResponse, MobileResponseDto>(response);
        //}

        //[AllowAnonymous]
        //public virtual async Task<MobileResponseDto> ConfirmPasswordResetAsync(PasswordResetConfirmDto input)
        //{
        //    var user = await _userManager.FindByEmailAsync(input.Email);
        //    if (user == null)
        //    {
        //        throw new UserFriendlyException("User not found with this email address.");
        //    }

        //    // Verify security code from cache
        //    var cacheKey = $"password_reset_{user.Id}";
        //    var cacheItem = await _downloadTokenCache.GetAsync(cacheKey);

        //    if (cacheItem == null || cacheItem.SecurityCode != input.SecurityCode)
        //    {
        //        throw new UserFriendlyException("Invalid or expired security code.");
        //    }

        //    // Generate reset token and reset password
        //    var token = await _userManager.GeneratePasswordResetTokenForEmailAsync(input.Email);
        //    await _userManager.ResetPasswordForEmailAsync(input.Email, token, input.NewPassword);

        //    // Remove the security code from cache
        //    await _downloadTokenCache.RemoveAsync(cacheKey);

        //    var response = new MobileResponse
        //    {
        //        Code = 200,
        //        Message = "Password reset successfully",
        //        Data = null
        //    };

        //    return ObjectMapper.Map<MobileResponse, MobileResponseDto>(response);
        //}

        [AllowAnonymous]
        public virtual async Task<MobileResponseDto> ResetPasswordWithoutTokenAsync(PasswordResetRequestDto input)
        {
            var user = await _userManager.FindByEmailAsync(input.Email);
            if (user == null)
            {
                throw new UserFriendlyException("User not found with this email address.");
            }


            // Generate reset token and reset password
            await _userManager.ResetPasswordWithoutTokenAsync(input.Email, input.NewPassword);

            var response = new MobileResponse
            {
                Code = 200,
                Message = "Password reset successfully",
                Data = null
            };

            return ObjectMapper.Map<MobileResponse, MobileResponseDto>(response);
        }

        [AllowAnonymous]
        public virtual async Task<MobileResponseDto> ChangePasswordAsync(PasswordChangeRequestDto input)
        {
            var user = await _userManager.FindByEmailAsync(input.Email);
            if (user == null)
            {
                throw new UserFriendlyException("User not found with this email address.");
            }

            // Generate reset token and reset password
            await _userManager.ChangePasswordAsync(input.Email,input.OldPassword, input.NewPassword);

            var response = new MobileResponse
            {
                Code = 200,
                Message = "Password reset successfully",
                Data = null
            };

            return ObjectMapper.Map<MobileResponse, MobileResponseDto>(response);
        }

        [AllowAnonymous]
        public virtual async Task<MobileResponseDto> ResendSecurityCodeAsync(SecurityNumberCreateDto input)
        {
            return null;
        }

        [Authorize(ImaarPermissions.UserProfiles.Default)]
        public virtual async Task<UserProfileWithDetailsDto> GetWithDetailsAsync(Guid id)
        {
            return ObjectMapper.Map<UserProfileWithDetails, UserProfileWithDetailsDto>(await _userProfileRepository.GetWithDetailsAsync(id));
        }
    }
}