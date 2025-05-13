using System;

namespace Imaar.Medias
{
    public abstract class MediaExcelDtoBase
    {
        public string? Title { get; set; }
        public string File { get; set; } = null!;
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}