using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Imaar.Tickets
{
    public abstract class TicketUpdateDtoBase : IHasConcurrencyStamp
    {
        [Required]
        public string Description { get; set; } = null!;
        public Guid TicketTypeId { get; set; }
        public Guid TicketCreatorId { get; set; }
        public Guid TicketAgainstId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}