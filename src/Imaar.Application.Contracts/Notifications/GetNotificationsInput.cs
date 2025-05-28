using Imaar.Notifications;
using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.Notifications
{
    public abstract class GetNotificationsInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Title { get; set; }
        public string? Message { get; set; }
        public bool? IsRead { get; set; }
        public DateTime? ReadDateMin { get; set; }
        public DateTime? ReadDateMax { get; set; }
        public int? PriorityMin { get; set; }
        public int? PriorityMax { get; set; }
        public SourceEntityType? SourceEntityType { get; set; }
        public string? SourceEntityId { get; set; }
        public string? SenderUserId { get; set; }
        public Guid? UserProfileId { get; set; }
        public Guid? NotificationTypeId { get; set; }

        public GetNotificationsInputBase()
        {

        }
    }
}