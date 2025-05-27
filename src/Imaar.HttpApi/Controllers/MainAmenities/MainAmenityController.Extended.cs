using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.MainAmenities;

namespace Imaar.Controllers.MainAmenities
{
    [RemoteService]
    [Area("app")]
    [ControllerName("MainAmenity")]
    [Route("api/mobile/main-amenities")]

    public class MainAmenityController : MainAmenityControllerBase, IMainAmenitiesAppService
    {
        public MainAmenityController(IMainAmenitiesAppService mainAmenitiesAppService) : base(mainAmenitiesAppService)
        {
        }
    }
}