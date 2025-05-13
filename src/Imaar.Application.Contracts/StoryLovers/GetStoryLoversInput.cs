using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.StoryLovers
{
    public abstract class GetStoryLoversInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public Guid? UserProfileId { get; set; }
        public Guid? StoryId { get; set; }

        public GetStoryLoversInputBase()
        {

        }
    }
}