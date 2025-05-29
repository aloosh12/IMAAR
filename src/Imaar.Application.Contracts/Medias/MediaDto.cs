using Imaar.Medias;
using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Imaar.Medias
{
    public abstract class MediaDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string? Title { get; set; }
        public string File { get; set; } = null!;
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public MediaEntityType SourceEntityType { get; set; }
        public string SourceEntityId { get; set; } = null!;

        public string ConcurrencyStamp { get; set; } = null!;

    }
}