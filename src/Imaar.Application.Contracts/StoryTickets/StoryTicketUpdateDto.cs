using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Imaar.StoryTickets
{
    public abstract class StoryTicketUpdateDtoBase : IHasConcurrencyStamp
    {
        [Required]
        public string Description { get; set; } = null!;
        public Guid StoryTicketTypeId { get; set; }
        public Guid TicketCreatorId { get; set; }
        public Guid StoryAgainstId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}