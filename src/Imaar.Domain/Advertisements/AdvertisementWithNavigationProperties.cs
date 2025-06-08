using Imaar.UserProfiles;

using System;
using System.Collections.Generic;

namespace Imaar.Advertisements
{
    public abstract class AdvertisementWithNavigationPropertiesBase
    {
        public Advertisement Advertisement { get; set; } = null!;

        public UserProfile UserProfile { get; set; } = null!;
        

        
    }
}