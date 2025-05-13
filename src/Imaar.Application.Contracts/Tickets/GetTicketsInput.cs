using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.Tickets
{
    public abstract class GetTicketsInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Description { get; set; }
        public Guid? TicketTypeId { get; set; }
        public Guid? TicketCreatorId { get; set; }
        public Guid? TicketAgainstId { get; set; }

        public GetTicketsInputBase()
        {

        }
    }
}