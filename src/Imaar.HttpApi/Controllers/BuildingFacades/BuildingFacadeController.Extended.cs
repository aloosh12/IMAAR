using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.BuildingFacades;

namespace Imaar.Controllers.BuildingFacades
{
    [RemoteService]
    [Area("app")]
    [ControllerName("BuildingFacade")]
    [Route("api/mobile/building-facades")]

    public class BuildingFacadeController : BuildingFacadeControllerBase, IBuildingFacadesAppService
    {
        public BuildingFacadeController(IBuildingFacadesAppService buildingFacadesAppService) : base(buildingFacadesAppService)
        {
        }
    }
}