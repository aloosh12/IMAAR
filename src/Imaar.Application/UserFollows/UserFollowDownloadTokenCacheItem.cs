using System;

namespace Imaar.UserFollows;

public abstract class UserFollowDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}