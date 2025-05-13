using Imaar.UserProfiles;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace Imaar.Stories
{
    public abstract class StoryWithNavigationPropertiesDtoBase
    {
        public StoryDto Story { get; set; } = null!;

        public UserProfileDto StoryPublisher { get; set; } = null!;

    }
}