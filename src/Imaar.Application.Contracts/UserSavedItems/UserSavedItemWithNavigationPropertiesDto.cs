using Imaar.UserProfiles;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace Imaar.UserSavedItems
{
    public abstract class UserSavedItemWithNavigationPropertiesDtoBase
    {
        public UserSavedItemDto UserSavedItem { get; set; } = null!;

        public UserProfileDto UserProfile { get; set; } = null!;

    }
}