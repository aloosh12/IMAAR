using Imaar.UserProfiles;

using System;
using System.Collections.Generic;

namespace Imaar.UserWorksExhibitions
{
    public abstract class UserWorksExhibitionWithNavigationPropertiesBase
    {
        public UserWorksExhibition UserWorksExhibition { get; set; } = null!;

        public UserProfile UserProfile { get; set; } = null!;
        

        
    }
}