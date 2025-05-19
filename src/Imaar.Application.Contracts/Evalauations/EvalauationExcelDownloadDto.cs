using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.Evalauations
{
    public abstract class EvalauationExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public int? SpeedOfCompletionMin { get; set; }
        public int? SpeedOfCompletionMax { get; set; }
        public int? DealingMin { get; set; }
        public int? DealingMax { get; set; }
        public int? CleanlinessMin { get; set; }
        public int? CleanlinessMax { get; set; }
        public int? PerfectionMin { get; set; }
        public int? PerfectionMax { get; set; }
        public int? PriceMin { get; set; }
        public int? PriceMax { get; set; }
        public Guid? Evaluatord { get; set; }
        public Guid? EvaluatedPersonId { get; set; }

        public EvalauationExcelDownloadDtoBase()
        {

        }
    }
}