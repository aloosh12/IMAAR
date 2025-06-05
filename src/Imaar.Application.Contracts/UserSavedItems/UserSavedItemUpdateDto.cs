using Imaar.UserSavedItems;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Imaar.UserSavedItems
{
    public abstract class UserSavedItemUpdateDtoBase : IHasConcurrencyStamp
    {
        [Required]
        public string SourceId { get; set; } = null!;
        public UserSavedItemType SavedItemType { get; set; }
        public Guid UserProfileId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}