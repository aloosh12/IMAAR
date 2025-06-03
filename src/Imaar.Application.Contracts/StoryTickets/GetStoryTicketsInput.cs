using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.StoryTickets
{
    public abstract class GetStoryTicketsInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Description { get; set; }
        public Guid? StoryTicketTypeId { get; set; }
        public Guid? TicketCreatorId { get; set; }
        public Guid? StoryAgainstId { get; set; }

        public GetStoryTicketsInputBase()
        {

        }
    }
}