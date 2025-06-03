using System;

namespace Imaar.StoryTickets;

public abstract class StoryTicketDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}