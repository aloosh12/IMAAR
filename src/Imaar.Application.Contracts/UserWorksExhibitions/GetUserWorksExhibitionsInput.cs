using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.UserWorksExhibitions
{
    public abstract class GetUserWorksExhibitionsInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Title { get; set; }
        public string? File { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public Guid? UserProfileId { get; set; }

        public GetUserWorksExhibitionsInputBase()
        {

        }
    }
}