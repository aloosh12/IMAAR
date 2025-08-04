using Imaar.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.UserSavedItems;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.UserSavedItems
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserSavedItem")]
    [Route("api/app/imaar-services")]

    public abstract class UserSavedItemControllerBase : AbpController
    {
        protected IUserSavedItemsAppService _userSavedItemsAppService;

        public UserSavedItemControllerBase(IUserSavedItemsAppService userSavedItemsAppService)
        {
            _userSavedItemsAppService = userSavedItemsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<UserSavedItemWithNavigationPropertiesDto>> GetListAsync(GetUserSavedItemsInput input)
        {
            return _userSavedItemsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<UserSavedItemWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _userSavedItemsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<UserSavedItemDto> GetAsync(Guid id)
        {
            return _userSavedItemsAppService.GetAsync(id);
        }



        [HttpGet]
        [Route("user-profile-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input)
        {
            return _userSavedItemsAppService.GetUserProfileLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<UserSavedItemDto> CreateAsync(UserSavedItemCreateDto input)
        {
            return _userSavedItemsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<UserSavedItemDto> UpdateAsync(Guid id, UserSavedItemUpdateDto input)
        {
            return _userSavedItemsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _userSavedItemsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(UserSavedItemExcelDownloadDto input)
        {
            return _userSavedItemsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _userSavedItemsAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> imaarserviceIds)
        {
            return _userSavedItemsAppService.DeleteByIdsAsync(imaarserviceIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetUserSavedItemsInput input)
        {
            return _userSavedItemsAppService.DeleteAllAsync(input);
        }
    }
}