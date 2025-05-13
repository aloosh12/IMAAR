using Imaar.UserProfiles;
using Imaar.Stories;

using System;
using System.Collections.Generic;

namespace Imaar.StoryLovers
{
    public abstract class StoryLoverWithNavigationPropertiesBase
    {
        public StoryLover StoryLover { get; set; } = null!;

        public UserProfile UserProfile { get; set; } = null!;
        public Story Story { get; set; } = null!;
        

        
    }
}