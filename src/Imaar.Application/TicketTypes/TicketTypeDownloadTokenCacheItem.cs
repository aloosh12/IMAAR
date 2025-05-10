using System;

namespace Imaar.TicketTypes;

public abstract class TicketTypeDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}