using Imaar.TicketTypes;
using Imaar.UserProfiles;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace Imaar.Tickets
{
    public abstract class TicketWithNavigationPropertiesDtoBase
    {
        public TicketDto Ticket { get; set; } = null!;

        public TicketTypeDto TicketType { get; set; } = null!;
        public UserProfileDto TicketCreator { get; set; } = null!;
        public UserProfileDto TicketAgainst { get; set; } = null!;

    }
}