using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.Tickets
{
    public abstract class TicketExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public string? Description { get; set; }
        public Guid? TicketTypeId { get; set; }
        public Guid? TicketCreatorId { get; set; }
        public Guid? TicketAgainstId { get; set; }

        public TicketExcelDownloadDtoBase()
        {

        }
    }
}