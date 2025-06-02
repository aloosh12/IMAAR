using Imaar.Notifications;
using System;
using Volo.Abp.Application.Dtos;

namespace Imaar.ImaarServices
{
    public class ImaarServiceShopListItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string MediaName { get; set; }

        public SourceEntityType SourceEntityType { get; set; }
    }
} 