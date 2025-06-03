using Imaar.StoryTicketTypes;
using Imaar.UserProfiles;
using Imaar.Stories;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace Imaar.StoryTickets
{
    public abstract class StoryTicketWithNavigationPropertiesDtoBase
    {
        public StoryTicketDto StoryTicket { get; set; } = null!;

        public StoryTicketTypeDto StoryTicketType { get; set; } = null!;
        public UserProfileDto TicketCreator { get; set; } = null!;
        public StoryDto StoryAgainst { get; set; } = null!;

    }
}