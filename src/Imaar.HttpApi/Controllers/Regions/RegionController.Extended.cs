using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.Regions;

namespace Imaar.Controllers.Regions
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Region")]
    [Route("api/mobile/regions")]

    public class RegionController : RegionControllerBase, IRegionsAppService
    {
        public RegionController(IRegionsAppService regionsAppService) : base(regionsAppService)
        {
        }
    }
}