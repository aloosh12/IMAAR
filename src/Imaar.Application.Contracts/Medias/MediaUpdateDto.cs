using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Imaar.Medias
{
    public abstract class MediaUpdateDtoBase : IHasConcurrencyStamp
    {
        public string? Title { get; set; }
        [Required]
        public string File { get; set; } = null!;
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public Guid? ImaarServiceId { get; set; }
        public Guid? VacancyId { get; set; }
        public Guid? StoryId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}