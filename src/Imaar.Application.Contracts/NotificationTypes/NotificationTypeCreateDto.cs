using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.NotificationTypes
{
    public abstract class NotificationTypeCreateDtoBase
    {
        [Required]
        public string Title { get; set; } = null!;
    }
}