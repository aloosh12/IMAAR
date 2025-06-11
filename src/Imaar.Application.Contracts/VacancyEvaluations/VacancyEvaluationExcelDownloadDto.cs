using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.VacancyEvaluations
{
    public abstract class VacancyEvaluationExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public int? RateMin { get; set; }
        public int? RateMax { get; set; }
        public Guid? UserProfileId { get; set; }
        public Guid? VacancyId { get; set; }

        public VacancyEvaluationExcelDownloadDtoBase()
        {

        }
    }
}