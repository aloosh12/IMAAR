using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Imaar.VacancyEvaluations
{
    public abstract class VacancyEvaluationUpdateDtoBase : IHasConcurrencyStamp
    {
        public int Rate { get; set; }
        public Guid UserProfileId { get; set; }
        public Guid VacancyId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}