using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.VacancyAdditionalFeatures
{
    public abstract class VacancyAdditionalFeatureCreateDtoBase
    {
        [Required]
        public string Name { get; set; } = null!;
        public int Order { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}