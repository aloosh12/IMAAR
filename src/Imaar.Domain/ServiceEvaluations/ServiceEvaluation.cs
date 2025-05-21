using Imaar.UserProfiles;
using Imaar.ImaarServices;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace Imaar.ServiceEvaluations
{
    public abstract class ServiceEvaluationBase : FullAuditedAggregateRoot<Guid>
    {
        public virtual int Rate { get; set; }
        public Guid EvaluatorId { get; set; }
        public Guid ImaarServiceId { get; set; }

        protected ServiceEvaluationBase()
        {

        }

        public ServiceEvaluationBase(Guid id, Guid evaluatorId, Guid imaarServiceId, int rate)
        {

            Id = id;
            Rate = rate;
            EvaluatorId = evaluatorId;
            ImaarServiceId = imaarServiceId;
        }

    }
}