using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.ServiceEvaluations
{
    public abstract class GetServiceEvaluationsInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public int? RateMin { get; set; }
        public int? RateMax { get; set; }
        public Guid? EvaluatorId { get; set; }
        public Guid? ImaarServiceId { get; set; }

        public GetServiceEvaluationsInputBase()
        {

        }
    }
}