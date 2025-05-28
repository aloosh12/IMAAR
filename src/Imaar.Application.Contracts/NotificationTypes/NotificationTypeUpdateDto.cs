using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Imaar.NotificationTypes
{
    public abstract class NotificationTypeUpdateDtoBase : IHasConcurrencyStamp
    {
        [Required]
        public string Title { get; set; } = null!;

        public string ConcurrencyStamp { get; set; } = null!;
    }
}