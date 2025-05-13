using System;

namespace Imaar.Tickets;

public abstract class TicketDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}