using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Imaar.Buildings
{
    public abstract class BuildingDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string MainTitle { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Price { get; set; } = null!;
        public string BuildingArea { get; set; } = null!;
        public string NumberOfRooms { get; set; } = null!;
        public string NumberOfBaths { get; set; } = null!;
        public string FloorNo { get; set; } = null!;
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public int ViewCounter { get; set; }
        public int OrderCounter { get; set; }
        public Guid RegionId { get; set; }
        public Guid FurnishingLevelId { get; set; }
        public Guid BuildingFacadeId { get; set; }
        public Guid ServiceTypeId { get; set; }
        public Guid UserProfileId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}