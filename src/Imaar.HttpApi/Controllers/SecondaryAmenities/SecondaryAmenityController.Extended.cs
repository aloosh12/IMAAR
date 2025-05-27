using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.SecondaryAmenities;

namespace Imaar.Controllers.SecondaryAmenities
{
    [RemoteService]
    [Area("app")]
    [ControllerName("SecondaryAmenity")]
    [Route("api/mobile/secondary-amenities")]

    public class SecondaryAmenityController : SecondaryAmenityControllerBase, ISecondaryAmenitiesAppService
    {
        public SecondaryAmenityController(ISecondaryAmenitiesAppService secondaryAmenitiesAppService) : base(secondaryAmenitiesAppService)
        {
        }
    }
}