using Imaar.ImaarServices;
using Imaar.Vacancies;
using Imaar.Stories;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace Imaar.Medias
{
    public abstract class MediaWithNavigationPropertiesDtoBase
    {
        public MediaDto Media { get; set; } = null!;

        public ImaarServiceDto ImaarService { get; set; } = null!;
        public VacancyDto Vacancy { get; set; } = null!;
        public StoryDto Story { get; set; } = null!;

    }
}