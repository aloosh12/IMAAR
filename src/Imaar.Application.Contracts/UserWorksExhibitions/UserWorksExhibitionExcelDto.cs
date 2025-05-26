using System;

namespace Imaar.UserWorksExhibitions
{
    public abstract class UserWorksExhibitionExcelDtoBase
    {
        public string? Title { get; set; }
        public string File { get; set; } = null!;
        public int Order { get; set; }
    }
}