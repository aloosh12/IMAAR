using Imaar.UserProfiles;
using Imaar.Vacancies;

using System;
using System.Collections.Generic;

namespace Imaar.VacancyEvaluations
{
    public abstract class VacancyEvaluationWithNavigationPropertiesBase
    {
        public VacancyEvaluation VacancyEvaluation { get; set; } = null!;

        public UserProfile UserProfile { get; set; } = null!;
        public Vacancy Vacancy { get; set; } = null!;
        

        
    }
}