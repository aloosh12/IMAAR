using System;

namespace Imaar.StoryTicketTypes
{
    public abstract class StoryTicketTypeExcelDtoBase
    {
        public string Title { get; set; } = null!;
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}