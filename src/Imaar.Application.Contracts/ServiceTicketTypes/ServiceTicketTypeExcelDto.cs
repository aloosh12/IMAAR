using System;

namespace Imaar.ServiceTicketTypes
{
    public abstract class ServiceTicketTypeExcelDtoBase
    {
        public string Title { get; set; } = null!;
        public int? Order { get; set; }
        public bool IsActive { get; set; }
    }
}