using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.Medias
{
    public abstract class GetMediasInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Title { get; set; }
        public string? File { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public bool? IsActive { get; set; }
        public Guid? ImaarServiceId { get; set; }
        public Guid? VacancyId { get; set; }
        public Guid? StoryId { get; set; }

        public GetMediasInputBase()
        {

        }
    }
}