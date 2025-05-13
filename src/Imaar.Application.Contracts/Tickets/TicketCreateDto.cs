using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.Tickets
{
    public abstract class TicketCreateDtoBase
    {
        [Required]
        public string Description { get; set; } = null!;
        public Guid TicketTypeId { get; set; }
        public Guid TicketCreatorId { get; set; }
        public Guid TicketAgainstId { get; set; }
    }
}