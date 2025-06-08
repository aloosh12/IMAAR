using System;

namespace Imaar.VacancyAdditionalFeatures
{
    public abstract class VacancyAdditionalFeatureExcelDtoBase
    {
        public string Name { get; set; } = null!;
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}