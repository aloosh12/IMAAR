using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Imaar.Stories
{
    public abstract class StoryUpdateDtoBase : IHasConcurrencyStamp
    {
        public string? Title { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ExpiryTime { get; set; }
        public Guid? StoryPublisherId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}