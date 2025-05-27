using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.UserFollows
{
    public abstract class UserFollowExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public Guid? FollowerUserId { get; set; }
        public Guid? FollowingUserId { get; set; }

        public UserFollowExcelDownloadDtoBase()
        {

        }
    }
}