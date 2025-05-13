using Imaar.ServiceTypes;
using Imaar.UserProfiles;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace Imaar.Vacancies
{
    public abstract class VacancyWithNavigationPropertiesDtoBase
    {
        public VacancyDto Vacancy { get; set; } = null!;

        public ServiceTypeDto ServiceType { get; set; } = null!;
        public UserProfileDto UserProfile { get; set; } = null!;

    }
}