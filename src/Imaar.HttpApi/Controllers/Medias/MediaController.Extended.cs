using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.Medias;

namespace Imaar.Controllers.Medias
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Media")]
    [Route("api/mobile/medias")]

    public class MediaController : MediaControllerBase, IMediasAppService
    {
        public MediaController(IMediasAppService mediasAppService) : base(mediasAppService)
        {
        }
    }
}