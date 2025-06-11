using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.BuildingEvaluations
{
    public abstract class GetBuildingEvaluationsInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public int? RateMin { get; set; }
        public int? RateMax { get; set; }
        public Guid? EvaluatorId { get; set; }
        public Guid? BuildingId { get; set; }

        public GetBuildingEvaluationsInputBase()
        {

        }
    }
}