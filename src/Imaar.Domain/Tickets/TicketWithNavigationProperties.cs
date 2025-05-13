using Imaar.TicketTypes;
using Imaar.UserProfiles;
using Imaar.UserProfiles;

using System;
using System.Collections.Generic;

namespace Imaar.Tickets
{
    public abstract class TicketWithNavigationPropertiesBase
    {
        public Ticket Ticket { get; set; } = null!;

        public TicketType TicketType { get; set; } = null!;
        public UserProfile TicketCreator { get; set; } = null!;
        public UserProfile TicketAgainst { get; set; } = null!;
        

        
    }
}