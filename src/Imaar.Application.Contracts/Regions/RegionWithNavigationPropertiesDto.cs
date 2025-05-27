using Imaar.Cities;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace Imaar.Regions
{
    public abstract class RegionWithNavigationPropertiesDtoBase
    {
        public RegionDto Region { get; set; } = null!;

        public CityDto City { get; set; } = null!;

    }
}