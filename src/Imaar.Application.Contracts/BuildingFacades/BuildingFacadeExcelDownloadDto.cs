using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.BuildingFacades
{
    public abstract class BuildingFacadeExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public string? Name { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public bool? IsActive { get; set; }

        public BuildingFacadeExcelDownloadDtoBase()
        {

        }
    }
}