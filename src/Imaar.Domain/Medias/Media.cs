using Imaar.ImaarServices;
using Imaar.Vacancies;
using Imaar.Stories;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace Imaar.Medias
{
    public abstract class MediaBase : FullAuditedAggregateRoot<Guid>
    {
        [CanBeNull]
        public virtual string? Title { get; set; }

        [NotNull]
        public virtual string File { get; set; }

        public virtual int Order { get; set; }

        public virtual bool IsActive { get; set; }
        public Guid? ImaarServiceId { get; set; }
        public Guid? VacancyId { get; set; }
        public Guid? StoryId { get; set; }

        protected MediaBase()
        {

        }

        public MediaBase(Guid id, Guid? imaarServiceId, Guid? vacancyId, Guid? storyId, string file, int order, bool isActive, string? title = null)
        {

            Id = id;
            Check.NotNull(file, nameof(file));
            File = file;
            Order = order;
            IsActive = isActive;
            Title = title;
            ImaarServiceId = imaarServiceId;
            VacancyId = vacancyId;
            StoryId = storyId;
        }

    }
}