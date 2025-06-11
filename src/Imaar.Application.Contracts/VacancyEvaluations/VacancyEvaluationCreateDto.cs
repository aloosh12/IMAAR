using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.VacancyEvaluations
{
    public abstract class VacancyEvaluationCreateDtoBase
    {
        public int Rate { get; set; } = 0;
        public Guid UserProfileId { get; set; }
        public Guid VacancyId { get; set; }
    }
}