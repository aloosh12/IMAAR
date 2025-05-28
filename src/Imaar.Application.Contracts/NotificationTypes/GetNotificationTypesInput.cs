using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.NotificationTypes
{
    public abstract class GetNotificationTypesInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Title { get; set; }

        public GetNotificationTypesInputBase()
        {

        }
    }
}