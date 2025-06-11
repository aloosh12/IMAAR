using Imaar.UserProfiles;
using Imaar.Vacancies;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace Imaar.VacancyEvaluations
{
    public abstract class VacancyEvaluationBase : FullAuditedAggregateRoot<Guid>
    {
        public virtual int Rate { get; set; }
        public Guid UserProfileId { get; set; }
        public Guid VacancyId { get; set; }

        protected VacancyEvaluationBase()
        {

        }

        public VacancyEvaluationBase(Guid id, Guid userProfileId, Guid vacancyId, int rate)
        {

            Id = id;
            Rate = rate;
            UserProfileId = userProfileId;
            VacancyId = vacancyId;
        }

    }
}