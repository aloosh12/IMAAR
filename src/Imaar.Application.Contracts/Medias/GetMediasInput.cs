using Imaar.Medias;
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
        public MediaEntityType? SourceEntityType { get; set; }
        public string? SourceEntityId { get; set; }

        public GetMediasInputBase()
        {

        }
    }
}