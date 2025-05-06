using System;

namespace Imaar.UserProfiles;

public abstract class UserProfileDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}