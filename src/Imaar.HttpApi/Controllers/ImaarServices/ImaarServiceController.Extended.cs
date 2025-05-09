using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.ImaarServices;

namespace Imaar.Controllers.ImaarServices
{
    [RemoteService]
    [Area("app")]
    [ControllerName("ImaarService")]
    [Route("api/mobile/imaar-services")]

    public class ImaarServiceController : ImaarServiceControllerBase, IImaarServicesAppService
    {
        public ImaarServiceController(IImaarServicesAppService imaarServicesAppService) : base(imaarServicesAppService)
        {
        }
    }
}