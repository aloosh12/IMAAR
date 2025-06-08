using Imaar.UserProfiles;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace Imaar.Advertisements
{
    public abstract class AdvertisementWithNavigationPropertiesDtoBase
    {
        public AdvertisementDto Advertisement { get; set; } = null!;

        public UserProfileDto UserProfile { get; set; } = null!;

    }
}