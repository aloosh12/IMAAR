using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.Evalauations;

namespace Imaar.Controllers.Evalauations
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Evalauation")]
    [Route("api/mobile/evalauations")]

    public class EvalauationController : EvalauationControllerBase, IEvalauationsAppService
    {
        public EvalauationController(IEvalauationsAppService evalauationsAppService) : base(evalauationsAppService)
        {
        }
    }
}