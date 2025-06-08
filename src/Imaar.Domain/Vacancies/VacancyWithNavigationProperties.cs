using Imaar.ServiceTypes;
using Imaar.UserProfiles;
using Imaar.VacancyAdditionalFeatures;

using System;
using System.Collections.Generic;

namespace Imaar.Vacancies
{
    public abstract class VacancyWithNavigationPropertiesBase
    {
        public Vacancy Vacancy { get; set; } = null!;

        public ServiceType ServiceType { get; set; } = null!;
        public UserProfile UserProfile { get; set; } = null!;
        

        public List<VacancyAdditionalFeature> VacancyAdditionalFeatures { get; set; } = null!;
        
    }
}