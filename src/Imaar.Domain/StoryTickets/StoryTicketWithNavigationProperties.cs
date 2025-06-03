using Imaar.StoryTicketTypes;
using Imaar.UserProfiles;
using Imaar.Stories;

using System;
using System.Collections.Generic;

namespace Imaar.StoryTickets
{
    public abstract class StoryTicketWithNavigationPropertiesBase
    {
        public StoryTicket StoryTicket { get; set; } = null!;

        public StoryTicketType StoryTicketType { get; set; } = null!;
        public UserProfile TicketCreator { get; set; } = null!;
        public Story StoryAgainst { get; set; } = null!;
        

        
    }
}