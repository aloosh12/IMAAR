using System;

namespace Imaar.BuildingFacades
{
    public abstract class BuildingFacadeExcelDtoBase
    {
        public string Name { get; set; } = null!;
        public int? Order { get; set; }
        public bool IsActive { get; set; }
    }
}