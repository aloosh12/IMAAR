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
        
        [HttpGet("with-details/{id}")]
        [AllowAnonymous]
        public virtual Task<MobileResponseDto> GetBuildingWithDetailsAsync(Guid id)
        {
            return _buildingsAppService.GetBuildingWithDetailsAsync(id);
        }
        
        [HttpPost("increment-view-counter/{id}")]
        [AllowAnonymous]
        public virtual Task<MobileResponseDto> IncrementViewCounterAsync(Guid id)
        {
            return _buildingsAppService.IncrementViewCounterAsync(id);
        }
        
        [HttpPost("increment-order-counter/{id}")]
        [AllowAnonymous]
        public virtual Task<MobileResponseDto> IncrementOrderCounterAsync(Guid id)
        {
            return _buildingsAppService.IncrementOrderCounterAsync(id);
        }
    }
}