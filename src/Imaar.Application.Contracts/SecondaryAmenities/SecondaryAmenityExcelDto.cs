using System;

namespace Imaar.SecondaryAmenities
{
    public abstract class SecondaryAmenityExcelDtoBase
    {
        public string Name { get; set; } = null!;
        public int? Order { get; set; }
        public bool IsActive { get; set; }
    }
}