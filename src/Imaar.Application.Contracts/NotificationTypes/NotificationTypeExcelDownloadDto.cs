using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.NotificationTypes
{
    public abstract class NotificationTypeExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public string? Title { get; set; }

        public NotificationTypeExcelDownloadDtoBase()
        {

        }
    }
}