using System.Collections.Generic;

namespace Imaar.Medias
{
    public class MediaBulkUpdateDto
    {
        public string SourceEntityId { get; set; } = null!;
        public MediaEntityType SourceEntityType { get; set; }
        public List<MediaDto> Medias { get; set; } = new List<MediaDto>();
    }
} 