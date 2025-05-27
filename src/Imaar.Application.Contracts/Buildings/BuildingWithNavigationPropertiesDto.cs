using Imaar.Regions;
using Imaar.FurnishingLevels;
using Imaar.BuildingFacades;
using Imaar.ServiceTypes;
using Imaar.MainAmenities;
using Imaar.SecondaryAmenities;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace Imaar.Buildings
{
    public abstract class BuildingWithNavigationPropertiesDtoBase
    {
        public BuildingDto Building { get; set; } = null!;

        public RegionDto Region { get; set; } = null!;
        public FurnishingLevelDto FurnishingLevel { get; set; } = null!;
        public BuildingFacadeDto BuildingFacade { get; set; } = null!;
        public ServiceTypeDto ServiceType { get; set; } = null!;
        public List<MainAmenityDto> MainAmenities { get; set; } = new List<MainAmenityDto>();
        public List<SecondaryAmenityDto> SecondaryAmenities { get; set; } = new List<SecondaryAmenityDto>();

    }
}