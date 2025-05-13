using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.Stories
{
    public abstract class StoryCreateDtoBase
    {
        public string? Title { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ExpiryTime { get; set; }
        public Guid? StoryPublisherId { get; set; }
    }
}