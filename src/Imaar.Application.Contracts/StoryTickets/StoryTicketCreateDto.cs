using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.StoryTickets
{
    public abstract class StoryTicketCreateDtoBase
    {
        [Required]
        public string Description { get; set; } = null!;
        public Guid StoryTicketTypeId { get; set; }
        public Guid TicketCreatorId { get; set; }
        public Guid StoryAgainstId { get; set; }
    }
}