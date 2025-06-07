using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.ServiceEvaluations;

namespace Imaar.Controllers.ServiceEvaluations
{
    [RemoteService]
    [Area("app")]
    [ControllerName("ServiceEvaluation")]
    [Route("api/mobile/service-evaluations")]

    public class ServiceEvaluationController : ServiceEvaluationControllerBase, IServiceEvaluationsAppService
    {
        public ServiceEvaluationController(IServiceEvaluationsAppService serviceEvaluationsAppService) : base(serviceEvaluationsAppService)
        {
        }

        [HttpGet("service-average-evaluation/{serviceId}")]
        public async Task<double> GetAverageEvaluationForServiceAsync(Guid serviceId)
        {
            return await  _serviceEvaluationsAppService.GetAverageEvaluationForServiceAsync(serviceId);    
        }
    }
}