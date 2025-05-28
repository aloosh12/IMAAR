using Imaar.ServiceTicketTypes;
using Imaar.UserProfiles;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace Imaar.ServiceTickets
{
    public abstract class ServiceTicketWithNavigationPropertiesDtoBase
    {
        public ServiceTicketDto ServiceTicket { get; set; } = null!;

        public ServiceTicketTypeDto ServiceTicketType { get; set; } = null!;
        public UserProfileDto TicketCreator { get; set; } = null!;

    }
}