using System;

namespace Imaar.Regions
{
    public abstract class RegionExcelDtoBase
    {
        public string Name { get; set; } = null!;
        public int? Order { get; set; }
        public bool IsActive { get; set; }
    }
}