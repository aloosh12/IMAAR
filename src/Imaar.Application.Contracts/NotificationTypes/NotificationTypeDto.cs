using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Imaar.NotificationTypes
{
    public abstract class NotificationTypeDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Title { get; set; } = null!;

        public string ConcurrencyStamp { get; set; } = null!;

    }
}