using System;

namespace Imaar.Cities
{
    public abstract class CityExcelDtoBase
    {
        public string Name { get; set; } = null!;
        public int? Order { get; set; }
        public bool IsActive { get; set; }
    }
}