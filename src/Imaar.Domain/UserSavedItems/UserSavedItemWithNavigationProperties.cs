using Imaar.UserProfiles;

using System;
using System.Collections.Generic;

namespace Imaar.UserSavedItems
{
    public abstract class UserSavedItemWithNavigationPropertiesBase
    {
        public UserSavedItem UserSavedItem { get; set; } = null!;

        public UserProfile UserProfile { get; set; } = null!;
        

        
    }
}