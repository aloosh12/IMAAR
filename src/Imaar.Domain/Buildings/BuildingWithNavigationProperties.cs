using Imaar.BuildingFacades;
using Imaar.FurnishingLevels;
using Imaar.MainAmenities;
using Imaar.Regions;
using Imaar.SecondaryAmenities;
using Imaar.ServiceTypes;
using Imaar.UserProfiles;
using System;
using System.Collections.Generic;

namespace Imaar.Buildings
{
    public abstract class BuildingWithNavigationPropertiesBase
    {
        public Building Building { get; set; } = null!;

        public Region Region { get; set; } = null!;
        public FurnishingLevel FurnishingLevel { get; set; } = null!;
        public BuildingFacade BuildingFacade { get; set; } = null!;
        public ServiceType ServiceType { get; set; } = null!;

        public UserProfile UserProfile { get; set; } = null!;
        public List<MainAmenity> MainAmenities { get; set; } = null!;
        public List<SecondaryAmenity> SecondaryAmenities { get; set; } = null!;
        
    }
}