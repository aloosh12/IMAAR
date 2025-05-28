using Imaar.ServiceTicketTypes;
using Imaar.UserProfiles;

using System;
using System.Collections.Generic;

namespace Imaar.ServiceTickets
{
    public abstract class ServiceTicketWithNavigationPropertiesBase
    {
        public ServiceTicket ServiceTicket { get; set; } = null!;

        public ServiceTicketType ServiceTicketType { get; set; } = null!;
        public UserProfile TicketCreator { get; set; } = null!;
        

        
    }
}