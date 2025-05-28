using System;

namespace Imaar.ServiceTicketTypes;

public abstract class ServiceTicketTypeDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}