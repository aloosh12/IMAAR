using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Imaar.StoryLovers
{
    public abstract class StoryLoverDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {

        public Guid UserProfileId { get; set; }
        public Guid StoryId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}