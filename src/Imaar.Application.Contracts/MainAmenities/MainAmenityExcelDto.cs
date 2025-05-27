using System;

namespace Imaar.MainAmenities
{
    public abstract class MainAmenityExcelDtoBase
    {
        public string Name { get; set; } = null!;
        public int? Order { get; set; }
        public bool IsActive { get; set; }
    }
}