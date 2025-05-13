using System;

namespace Imaar.Stories
{
    public abstract class StoryExcelDtoBase
    {
        public string? Title { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}