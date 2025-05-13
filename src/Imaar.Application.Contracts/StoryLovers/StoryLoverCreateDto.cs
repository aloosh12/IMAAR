using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.StoryLovers
{
    public abstract class StoryLoverCreateDtoBase
    {

        public Guid UserProfileId { get; set; }
        public Guid StoryId { get; set; }
    }
}