using Imaar.UserProfiles;
using Imaar.Vacancies;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace Imaar.VacancyEvaluations
{
    public abstract class VacancyEvaluationWithNavigationPropertiesDtoBase
    {
        public VacancyEvaluationDto VacancyEvaluation { get; set; } = null!;

        public UserProfileDto UserProfile { get; set; } = null!;
        public VacancyDto Vacancy { get; set; } = null!;

    }
}