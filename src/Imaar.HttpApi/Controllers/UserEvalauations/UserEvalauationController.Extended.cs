using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.UserEvalauations;

namespace Imaar.Controllers.UserEvalauations
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserEvalauation")]
    [Route("api/mobile/user-evalauations")]

    public class UserEvalauationController : UserEvalauationControllerBase, IUserEvalauationsAppService
    {
        public UserEvalauationController(IUserEvalauationsAppService userEvalauationsAppService) : base(userEvalauationsAppService)
        {
        }
    }
}