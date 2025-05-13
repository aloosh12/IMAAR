using Imaar.UserProfiles;

using System;
using System.Collections.Generic;

namespace Imaar.Stories
{
    public abstract class StoryWithNavigationPropertiesBase
    {
        public Story Story { get; set; } = null!;

        public UserProfile StoryPublisher { get; set; } = null!;
        

        
    }
}