using Imaar.UserSavedItems;
using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.UserSavedItems
{
    public abstract class GetUserSavedItemsInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? SourceId { get; set; }
        public UserSavedItemType? SavedItemType { get; set; }
        public Guid? UserProfileId { get; set; }

        public GetUserSavedItemsInputBase()
        {

        }
    }
}