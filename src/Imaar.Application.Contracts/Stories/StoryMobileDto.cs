using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Imaar.Stories
{
    public  class StoryMobileDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ExpiryTime { get; set; }
        public string StoryPublisher { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}