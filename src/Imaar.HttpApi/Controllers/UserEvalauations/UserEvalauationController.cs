using Imaar.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.UserEvalauations;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.UserEvalauations
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserEvalauation")]
    [Route("api/app/user-evalauations")]

    public abstract class UserEvalauationControllerBase : AbpController
    {
        protected IUserEvalauationsAppService _userEvalauationsAppService;

        public UserEvalauationControllerBase(IUserEvalauationsAppService userEvalauationsAppService)
        {
            _userEvalauationsAppService = userEvalauationsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<UserEvalauationWithNavigationPropertiesDto>> GetListAsync(GetUserEvalauationsInput input)
        {
            return _userEvalauationsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<UserEvalauationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _userEvalauationsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<UserEvalauationDto> GetAsync(Guid id)
        {
            return _userEvalauationsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("user-profile-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input)
        {
            return _userEvalauationsAppService.GetUserProfileLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<UserEvalauationDto> CreateAsync(UserEvalauationCreateDto input)
        {
            return _userEvalauationsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<UserEvalauationDto> UpdateAsync(Guid id, UserEvalauationUpdateDto input)
        {
            return _userEvalauationsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _userEvalauationsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(UserEvalauationExcelDownloadDto input)
        {
            return _userEvalauationsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _userEvalauationsAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> userEvalauationIds)
        {
            return _userEvalauationsAppService.DeleteByIdsAsync(userEvalauationIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetUserEvalauationsInput input)
        {
            return _userEvalauationsAppService.DeleteAllAsync(input);
        }
    }
}