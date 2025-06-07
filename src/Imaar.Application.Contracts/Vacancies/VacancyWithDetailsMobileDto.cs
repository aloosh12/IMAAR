using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Imaar.Medias;
using Imaar.ServiceTypes;
using Imaar.UserProfiles;

namespace Imaar.Vacancies
{
    public class VacancyWithDetailsMobileDto : VacancyDto
    {
        public ServiceTypeDto ServiceType { get; set; }
        public UserProfileWithDetailsDto UserProfileWithDetailsDto { get; set; }
        public List<MediaDto> Media { get; set; } = new();
    }
} 