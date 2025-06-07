using Imaar.UserProfiles;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;
using Imaar.Medias;

namespace Imaar.Stories
{
    public abstract class StoryWithNavigationPropertiesDtoBase
    {
        public StoryDto Story { get; set; } = null!;
        public UserProfileDto StoryPublisher { get; set; } = null!;
        public IReadOnlyList<MediaDto> MediaDtos { get; set; } = new List<MediaDto>();

    }
}