using Imaar.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.UserWorksExhibitions;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.UserWorksExhibitions
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserWorksExhibition")]
    [Route("api/app/user-works-exhibitions")]

    public abstract class UserWorksExhibitionControllerBase : AbpController
    {
        protected IUserWorksExhibitionsAppService _userWorksExhibitionsAppService;

        public UserWorksExhibitionControllerBase(IUserWorksExhibitionsAppService userWorksExhibitionsAppService)
        {
            _userWorksExhibitionsAppService = userWorksExhibitionsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<UserWorksExhibitionWithNavigationPropertiesDto>> GetListAsync(GetUserWorksExhibitionsInput input)
        {
            return _userWorksExhibitionsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<UserWorksExhibitionWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _userWorksExhibitionsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<UserWorksExhibitionDto> GetAsync(Guid id)
        {
            return _userWorksExhibitionsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("user-profile-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input)
        {
            return _userWorksExhibitionsAppService.GetUserProfileLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<UserWorksExhibitionDto> CreateAsync(UserWorksExhibitionCreateDto input)
        {
            return _userWorksExhibitionsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<UserWorksExhibitionDto> UpdateAsync(Guid id, UserWorksExhibitionUpdateDto input)
        {
            return _userWorksExhibitionsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _userWorksExhibitionsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(UserWorksExhibitionExcelDownloadDto input)
        {
            return _userWorksExhibitionsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _userWorksExhibitionsAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> userworksexhibitionIds)
        {
            return _userWorksExhibitionsAppService.DeleteByIdsAsync(userworksexhibitionIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetUserWorksExhibitionsInput input)
        {
            return _userWorksExhibitionsAppService.DeleteAllAsync(input);
        }
    }
}