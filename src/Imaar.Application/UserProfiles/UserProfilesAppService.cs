using AutoMapper.Internal.Mappers;
using Imaar.UserProfiles;
using Imaar.Permissions;
using Imaar.Shared;
using Imaar.UserProfiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Imaar.Permissions;
using Imaar.UserProfiles;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;
using Imaar.Authorizations;

namespace Imaar.UserProfiles
{

    [Authorize(ImaarPermissions.UserProfiles.Default)]
    public abstract class UserProfilesAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<UserProfileDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IUserProfileRepository _userProfileRepository;
        protected UserProfileManager _userProfileManager;
        protected UserManager _userManager;

        public UserProfilesAppServiceBase(IUserProfileRepository userProfileRepository, UserProfileManager userProfileManager, UserManager userManager, IDistributedCache<UserProfileDownloadTokenCacheItem, string> downloadTokenCache)
        {
            _downloadTokenCache = downloadTokenCache;
            _userProfileRepository = userProfileRepository;
            _userProfileManager = userProfileManager;
            _userManager = userManager;

        }

        public virtual async Task<PagedResultDto<UserProfileDto>> GetListAsync(GetUserProfilesInput input)
        {
            var totalCount = await _userProfileRepository.GetCountAsync(input.FilterText, input.SecurityNumber, input.BiologicalSex, input.DateOfBirthMin, input.DateOfBirthMax, input.Latitude, input.Longitude, input.FirstName, input.LastName, input.PhoneNumber, input.Email);
            var items = await _userProfileRepository.GetListAsync(input.FilterText, input.SecurityNumber, input.BiologicalSex, input.DateOfBirthMin, input.DateOfBirthMax, input.Latitude, input.Longitude, input.FirstName, input.LastName, input.PhoneNumber, input.Email, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<UserProfileDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<UserProfile>, List<UserProfileDto>>(items)
            };
        }

        public virtual async Task<UserProfileDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<UserProfile, UserProfileDto>(await _userProfileRepository.GetAsync(id));
        }

        [Authorize(ImaarPermissions.UserProfiles.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _userProfileRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.UserProfiles.Create)]
        public virtual async Task<UserProfileDto> CreateAsync(UserProfileCreateDto input)
        {

            var userProfile = await _userProfileManager.CreateAsync(
            input.SecurityNumber, input.FirstName, input.LastName, input.PhoneNumber, input.Email, input.BiologicalSex, input.DateOfBirth, input.Latitude, input.Longitude, input.ProfilePhoto
            );

            return ObjectMapper.Map<UserProfile, UserProfileDto>(userProfile);
        }

        [Authorize(ImaarPermissions.UserProfiles.Edit)]
        public virtual async Task<UserProfileDto> UpdateAsync(Guid id, UserProfileUpdateDto input)
        {

            var userProfile = await _userProfileManager.UpdateAsync(
            id,
            input.SecurityNumber, input.FirstName, input.LastName, input.PhoneNumber, input.Email, input.BiologicalSex, input.DateOfBirth, input.Latitude, input.Longitude, input.ProfilePhoto, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<UserProfile, UserProfileDto>(userProfile);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(UserProfileExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _userProfileRepository.GetListAsync(input.FilterText, input.SecurityNumber, input.BiologicalSex, input.DateOfBirthMin, input.DateOfBirthMax, input.Latitude, input.Longitude, input.FirstName, input.LastName, input.PhoneNumber, input.Email);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<UserProfile>, List<UserProfileExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "UserProfiles.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.UserProfiles.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> userprofileIds)
        {
            await _userProfileRepository.DeleteManyAsync(userprofileIds);
        }

        [Authorize(ImaarPermissions.UserProfiles.Delete)]
        public virtual async Task DeleteAllAsync(GetUserProfilesInput input)
        {
            await _userProfileRepository.DeleteAllAsync(input.FilterText, input.SecurityNumber, input.BiologicalSex, input.DateOfBirthMin, input.DateOfBirthMax, input.Latitude, input.Longitude, input.FirstName, input.LastName, input.PhoneNumber, input.Email);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new UserProfileDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new Imaar.Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}