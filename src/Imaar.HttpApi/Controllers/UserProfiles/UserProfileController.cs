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

namespace Imaar.Controllers.UserProfiles
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserProfile")]
    [Route("api/app/user-profiles")]

    public abstract class UserProfileControllerBase : AbpController
    {
        protected IUserProfilesAppService _userProfilesAppService;

        public UserProfileControllerBase(IUserProfilesAppService userProfilesAppService)
        {
            _userProfilesAppService = userProfilesAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<UserProfileDto>> GetListAsync(GetUserProfilesInput input)
        {
            return _userProfilesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<UserProfileDto> GetAsync(Guid id)
        {
            return _userProfilesAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<UserProfileDto> CreateAsync(UserProfileCreateDto input)
        {
            return _userProfilesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<UserProfileDto> UpdateAsync(Guid id, UserProfileUpdateDto input)
        {
            return _userProfilesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _userProfilesAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(UserProfileExcelDownloadDto input)
        {
            return _userProfilesAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _userProfilesAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> userprofileIds)
        {
            return _userProfilesAppService.DeleteByIdsAsync(userprofileIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetUserProfilesInput input)
        {
            return _userProfilesAppService.DeleteAllAsync(input);
        }
    }
}