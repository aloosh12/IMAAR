using Imaar.UserProfiles;
using Imaar.Stories;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace Imaar.StoryLovers
{
    public abstract class StoryLoverWithNavigationPropertiesDtoBase
    {
        public StoryLoverDto StoryLover { get; set; } = null!;

        public UserProfileDto UserProfile { get; set; } = null!;
        public StoryDto Story { get; set; } = null!;

    }
}