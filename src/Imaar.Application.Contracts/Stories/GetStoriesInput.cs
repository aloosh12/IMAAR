using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.Stories
{
    public abstract class GetStoriesInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Title { get; set; }
        public DateTime? FromTimeMin { get; set; }
        public DateTime? FromTimeMax { get; set; }
        public DateTime? ExpiryTimeMin { get; set; }
        public DateTime? ExpiryTimeMax { get; set; }
        public Guid? StoryPublisherId { get; set; }

        public GetStoriesInputBase()
        {

        }
    }
}