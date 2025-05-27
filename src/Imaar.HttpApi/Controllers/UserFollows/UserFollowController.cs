using Imaar.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.UserFollows;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.UserFollows
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserFollow")]
    [Route("api/app/user-follows")]

    public abstract class UserFollowControllerBase : AbpController
    {
        protected IUserFollowsAppService _userFollowsAppService;

        public UserFollowControllerBase(IUserFollowsAppService userFollowsAppService)
        {
            _userFollowsAppService = userFollowsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<UserFollowWithNavigationPropertiesDto>> GetListAsync(GetUserFollowsInput input)
        {
            return _userFollowsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<UserFollowWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _userFollowsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<UserFollowDto> GetAsync(Guid id)
        {
            return _userFollowsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("user-profile-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input)
        {
            return _userFollowsAppService.GetUserProfileLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<UserFollowDto> CreateAsync(UserFollowCreateDto input)
        {
            return _userFollowsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<UserFollowDto> UpdateAsync(Guid id, UserFollowUpdateDto input)
        {
            return _userFollowsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _userFollowsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(UserFollowExcelDownloadDto input)
        {
            return _userFollowsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _userFollowsAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> userfollowIds)
        {
            return _userFollowsAppService.DeleteByIdsAsync(userfollowIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetUserFollowsInput input)
        {
            return _userFollowsAppService.DeleteAllAsync(input);
        }
    }
}