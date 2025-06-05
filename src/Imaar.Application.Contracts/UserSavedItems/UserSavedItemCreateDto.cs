using Imaar.UserSavedItems;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.UserSavedItems
{
    public abstract class UserSavedItemCreateDtoBase
    {
        [Required]
        public string SourceId { get; set; } = null!;
        public UserSavedItemType SavedItemType { get; set; } = ((UserSavedItemType[])Enum.GetValues(typeof(UserSavedItemType)))[0];
        public Guid UserProfileId { get; set; }
    }
}