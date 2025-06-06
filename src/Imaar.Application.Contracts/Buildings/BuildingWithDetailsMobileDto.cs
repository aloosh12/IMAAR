using Imaar.BuildingFacades;
using Imaar.FurnishingLevels;
using Imaar.MainAmenities;
using Imaar.Medias;
using Imaar.Regions;
using Imaar.SecondaryAmenities;
using Imaar.ServiceTypes;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Imaar.Buildings
{
    public class BuildingWithDetailsMobileDto : BuildingWithNavigationPropertiesDtoBase
    {
        public List<MediaDto> Media { get; set; } = new();
    }
} 