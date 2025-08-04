using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.UserSavedItems;

namespace Imaar.Controllers.UserSavedItems
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserSavedItem")]
    [Route("api/mobile/user-saved-items")]

    public class UserSavedItemController : UserSavedItemControllerBase, IUserSavedItemsAppService
    {
        public UserSavedItemController(IUserSavedItemsAppService userSavedItemsAppService) : base(userSavedItemsAppService)
        {
        }
    }
}