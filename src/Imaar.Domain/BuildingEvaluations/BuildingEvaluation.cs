using Imaar.UserProfiles;
using Imaar.Buildings;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace Imaar.BuildingEvaluations
{
    public abstract class BuildingEvaluationBase : FullAuditedAggregateRoot<Guid>
    {
        public virtual int Rate { get; set; }
        public Guid EvaluatorId { get; set; }
        public Guid BuildingId { get; set; }

        protected BuildingEvaluationBase()
        {

        }

        public BuildingEvaluationBase(Guid id, Guid evaluatorId, Guid buildingId, int rate)
        {

            Id = id;
            Rate = rate;
            EvaluatorId = evaluatorId;
            BuildingId = buildingId;
        }

    }
}