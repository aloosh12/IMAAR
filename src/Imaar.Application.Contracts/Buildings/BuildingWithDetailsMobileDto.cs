using Imaar.BuildingFacades;
using Imaar.FurnishingLevels;
using Imaar.MainAmenities;
using Imaar.Medias;
using Imaar.Regions;
using Imaar.SecondaryAmenities;
using Imaar.ServiceTypes;
using Imaar.UserProfiles;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Imaar.Buildings
{
    public class BuildingWithDetailsMobileDto 
    {
        public BuildingDto Building { get; set; } = null!;

        public RegionDto Region { get; set; } = null!;
        public FurnishingLevelDto FurnishingLevel { get; set; } = null!;
        public BuildingFacadeDto BuildingFacade { get; set; } = null!;
        public ServiceTypeDto ServiceType { get; set; } = null!;
        public UserProfileWithDetailsDto UserProfileWithDetailsDto { get; set; } = null!;
        public List<MainAmenityDto> MainAmenities { get; set; } = new List<MainAmenityDto>();
        public List<SecondaryAmenityDto> SecondaryAmenities { get; set; } = new List<SecondaryAmenityDto>();
        public List<MediaDto> Media { get; set; } = new();
    }
} 