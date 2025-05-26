using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Imaar.UserWorksExhibitions
{
    public abstract class UserWorksExhibitionDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string? Title { get; set; }
        public string File { get; set; } = null!;
        public int Order { get; set; }
        public Guid UserProfileId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}