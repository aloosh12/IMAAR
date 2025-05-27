using System;

namespace Imaar.FurnishingLevels
{
    public abstract class FurnishingLevelExcelDtoBase
    {
        public string Name { get; set; } = null!;
        public int? Order { get; set; }
        public bool IsActive { get; set; }
    }
}