using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.ImaarServices;
using Microsoft.AspNetCore.Authorization;
using Imaar.MobileResponses;
using Imaar.Medias;
using System.Collections.Generic;

namespace Imaar.Controllers.ImaarServices
{
    [RemoteService]
    [Area("app")]
    [ControllerName("ImaarService")]
    [Route("api/mobile/imaar-services")]

    public class ImaarServiceController : ImaarServiceControllerBase, IImaarServicesAppService
    {
        private readonly IMediasAppService _mediasAppService;

        public ImaarServiceController(
            IImaarServicesAppService imaarServicesAppService,
            IMediasAppService mediasAppService) : base(imaarServicesAppService)
        {
            _mediasAppService = mediasAppService;
        }
        
        [HttpPost("create-with-files")]
        [AllowAnonymous]
        public virtual Task<MobileResponseDto> CreateWithFilesAsync([FromForm] ImaarServiceCreateWithFilesDto input)
        {
            return _imaarServicesAppService.CreateWithFilesAsync(input);
        }
        
        [HttpGet("shop-list")]
        [AllowAnonymous]
        public virtual Task<PagedResultDto<ImaarServiceShopListItemDto>> GetShopListAsync(ImaarServiceFilterDto input)
        {
            return _imaarServicesAppService.GetShopListAsync(input);
        }

        [HttpPost("update-medias")]
        [AllowAnonymous]
        public virtual async Task<List<MediaDto>> UpdateMediasAsync([FromBody] MediaBulkUpdateDto input)
        {
            return await _mediasAppService.BulkUpdateMediasAsync(input);
        }
        
        [HttpPost("increment-view-counter/{id}")]
        [AllowAnonymous]
        public virtual Task<MobileResponseDto> IncrementViewCounterAsync(Guid id)
        {
            return _imaarServicesAppService.IncrementViewCounterAsync(id);
        }
        
        [HttpPost("increment-order-counter/{id}")]
        [AllowAnonymous]
        public virtual Task<MobileResponseDto> IncrementOrderCounterAsync(Guid id)
        {
            return _imaarServicesAppService.IncrementOrderCounterAsync(id);
        }
    }
}