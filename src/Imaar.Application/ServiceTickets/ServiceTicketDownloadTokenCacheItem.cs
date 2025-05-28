using System;

namespace Imaar.ServiceTickets;

public abstract class ServiceTicketDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}