using Imaar.UserProfiles;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace Imaar.UserWorksExhibitions
{
    public abstract class UserWorksExhibitionWithNavigationPropertiesDtoBase
    {
        public UserWorksExhibitionDto UserWorksExhibition { get; set; } = null!;

        public UserProfileDto UserProfile { get; set; } = null!;

    }
}