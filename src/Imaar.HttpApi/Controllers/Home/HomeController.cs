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
using Imaar.Categories;
using Imaar.Pages;

namespace Imaar.Controllers.Home
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Home")]
    [Route("api/mobile/home")]

    public  class HomeController : AbpController
    {
        protected IHomeAppService _homeAppService;

        public HomeController(IHomeAppService homeAppService)
        {
            _homeAppService = homeAppService ;
        }

        [HttpGet("home")]
        public virtual Task<HomePageDto> GetListAsync()
        {
            return _homeAppService.GetAsync();        
        }


    }
}