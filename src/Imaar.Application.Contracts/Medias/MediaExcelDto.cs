using Imaar.Medias;
using System;

namespace Imaar.Medias
{
    public abstract class MediaExcelDtoBase
    {
        public string? Title { get; set; }
        public string File { get; set; } = null!;
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public MediaEntityType SourceEntityType { get; set; }
        public string SourceEntityId { get; set; } = null!;
    }
}