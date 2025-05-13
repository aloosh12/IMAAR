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
        public Guid? ImaarServiceId { get; set; }
        public Guid? VacancyId { get; set; }
        public Guid? StoryId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}