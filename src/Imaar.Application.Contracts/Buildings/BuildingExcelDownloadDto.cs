using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.Buildings
{
    public abstract class BuildingExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public string? MainTitle { get; set; }
        public string? Description { get; set; }
        public string? Price { get; set; }
        public string? BuildingArea { get; set; }
        public string? NumberOfRooms { get; set; }
        public string? NumberOfBaths { get; set; }
        public string? FloorNo { get; set; }
        public int? ViewCounterMin { get; set; }
        public int? ViewCounterMax { get; set; }
        public int? OrderCounterMin { get; set; }
        public int? OrderCounterMax { get; set; }
        public Guid? RegionId { get; set; }
        public Guid? FurnishingLevelId { get; set; }
        public Guid? BuildingFacadeId { get; set; }
        public Guid? ServiceTypeId { get; set; }
        public Guid? UserProfileId { get; set; }

        public Guid? MainAmenityId { get; set; }
        public Guid? SecondaryAmenityId { get; set; }

        public BuildingExcelDownloadDtoBase()
        {

        }
    }
}