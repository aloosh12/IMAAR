using Imaar.Cities;

using System;
using System.Collections.Generic;

namespace Imaar.Regions
{
    public abstract class RegionWithNavigationPropertiesBase
    {
        public Region Region { get; set; } = null!;

        public City City { get; set; } = null!;
        

        
    }
}