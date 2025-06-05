using Imaar.UserSavedItems;
using System;

namespace Imaar.UserSavedItems
{
    public abstract class UserSavedItemExcelDtoBase
    {
        public string SourceId { get; set; } = null!;
        public UserSavedItemType SavedItemType { get; set; }
    }
}