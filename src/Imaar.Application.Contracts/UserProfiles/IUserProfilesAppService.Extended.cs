using Imaar.MobileResponses;
using System;
using System.Threading.Tasks;

namespace Imaar.UserProfiles
{
    public partial interface IUserProfilesAppService
    {
        //Write your custom code here...
        Task<MobileResponseDto> RegisterAsync(RegisterCreateDto input);
        Task<MobileResponseDto> UpdateUserDetailsAsync(UserUpdateDto input);
        Task<UserProfileWithDetailsDto> GetWithDetailsAsync(Guid id);
        //Task<MobileResponseDto> RequestPasswordResetAsync(PasswordResetRequestDto input);
        //Task<MobileResponseDto> ConfirmPasswordResetAsync(PasswordResetConfirmDto input);
        Task<MobileResponseDto> VerifyEmailExistsAsync(VerifyEmailExistsDto input);
        Task<MobileResponseDto> ResetPasswordWithoutTokenAsync(PasswordResetRequestDto input);
        Task<MobileResponseDto> ChangePasswordAsync(PasswordChangeRequestDto input);
        
        Task<bool> CheckFollowStatusAsync(Guid id);
        
        Task<int> GetFollowCountAsync(Guid id);
    }
}