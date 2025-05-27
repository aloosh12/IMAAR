using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.Buildings;

namespace Imaar.Controllers.Buildings
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Building")]
    [Route("api/mobile/buildings")]

    public class BuildingController : BuildingControllerBase, IBuildingsAppService
    {
        public BuildingController(IBuildingsAppService buildingsAppService) : base(buildingsAppService)
        {
        }
    }
}