using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Imaar.VacancyAdditionalFeatures
{
    public abstract class VacancyAdditionalFeatureUpdateDtoBase : IHasConcurrencyStamp
    {
        [Required]
        public string Name { get; set; } = null!;
        public int Order { get; set; }
        public bool IsActive { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}