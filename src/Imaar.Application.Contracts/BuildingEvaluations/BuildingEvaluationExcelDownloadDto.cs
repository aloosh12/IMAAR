using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.BuildingEvaluations
{
    public abstract class BuildingEvaluationExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public int? RateMin { get; set; }
        public int? RateMax { get; set; }
        public Guid? EvaluatorId { get; set; }
        public Guid? BuildingId { get; set; }

        public BuildingEvaluationExcelDownloadDtoBase()
        {

        }
    }
}