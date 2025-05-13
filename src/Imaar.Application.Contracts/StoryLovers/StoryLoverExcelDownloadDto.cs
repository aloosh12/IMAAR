using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.StoryLovers
{
    public abstract class StoryLoverExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public Guid? UserProfileId { get; set; }
        public Guid? StoryId { get; set; }

        public StoryLoverExcelDownloadDtoBase()
        {

        }
    }
}