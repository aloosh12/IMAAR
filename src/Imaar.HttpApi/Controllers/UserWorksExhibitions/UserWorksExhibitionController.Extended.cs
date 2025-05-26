using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.UserWorksExhibitions;

namespace Imaar.Controllers.UserWorksExhibitions
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserWorksExhibition")]
    [Route("api/mobile/user-works-exhibitions")]

    public class UserWorksExhibitionController : UserWorksExhibitionControllerBase, IUserWorksExhibitionsAppService
    {
        public UserWorksExhibitionController(IUserWorksExhibitionsAppService userWorksExhibitionsAppService) : base(userWorksExhibitionsAppService)
        {
        }

        [HttpPost("create-mobile")]
        public virtual Task<UserWorksExhibitionDto> CreateMobileAsync(UserWorksExhibitionMobileCreateDto input)
        {
            return _userWorksExhibitionsAppService.CreateMobileAsync(input);
        }
    }
}