using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.Cities;

namespace Imaar.Controllers.Cities
{
    [RemoteService]
    [Area("app")]
    [ControllerName("City")]
    [Route("api/mobile/cities")]

    public class CityController : CityControllerBase, ICitiesAppService
    {
        public CityController(ICitiesAppService citiesAppService) : base(citiesAppService)
        {
        }
    }
}