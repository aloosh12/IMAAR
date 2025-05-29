using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.Buildings;
using Microsoft.AspNetCore.Authorization;
using Imaar.MobileResponses;

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
        
        [HttpPost("create-with-files")]
        [AllowAnonymous]
        public virtual Task<MobileResponseDto> CreateWithFilesAsync([FromForm] BuildingCreateWithFilesDto input)
        {
            return _buildingsAppService.CreateWithFilesAsync(input);
        }
    }
}