using Imaar.UserProfiles;
using Imaar.Medias;
using System.Collections.Generic;

namespace Imaar.Stories
{
    public class StoryWithNavigationProperties : StoryWithNavigationPropertiesBase
    {
        public List<Media> Medias { get; set; } = new List<Media>();
    }
}