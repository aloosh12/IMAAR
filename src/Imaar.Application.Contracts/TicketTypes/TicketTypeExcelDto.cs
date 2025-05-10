using System;

namespace Imaar.TicketTypes
{
    public abstract class TicketTypeExcelDtoBase
    {
        public string Title { get; set; } = null!;
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}