using Imaar.UserSavedItems;
using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Imaar.UserSavedItems
{
    public abstract class UserSavedItemDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string SourceId { get; set; } = null!;
        public UserSavedItemType SavedItemType { get; set; }
        public Guid UserProfileId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}