using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.FurnishingLevels;

namespace Imaar.Controllers.FurnishingLevels
{
    [RemoteService]
    [Area("app")]
    [ControllerName("FurnishingLevel")]
    [Route("api/mobile/furnishing-levels")]

    public class FurnishingLevelController : FurnishingLevelControllerBase, IFurnishingLevelsAppService
    {
        public FurnishingLevelController(IFurnishingLevelsAppService furnishingLevelsAppService) : base(furnishingLevelsAppService)
        {
        }
    }
}