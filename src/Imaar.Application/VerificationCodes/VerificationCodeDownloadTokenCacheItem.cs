using System;

namespace Imaar.VerificationCodes;

public abstract class VerificationCodeDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}