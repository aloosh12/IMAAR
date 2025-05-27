using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.UserFollows;

namespace Imaar.Controllers.UserFollows
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserFollow")]
    [Route("api/mobile/user-follows")]

    public class UserFollowController : UserFollowControllerBase, IUserFollowsAppService
    {
        public UserFollowController(IUserFollowsAppService userFollowsAppService) : base(userFollowsAppService)
        {
        }
    }
}