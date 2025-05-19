using Imaar.UserProfiles;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace Imaar.Evalauations
{
    public abstract class EvalauationBase : FullAuditedAggregateRoot<Guid>
    {
        public virtual int SpeedOfCompletion { get; set; }

        public virtual int Dealing { get; set; }

        public virtual int Cleanliness { get; set; }

        public virtual int Perfection { get; set; }

        public virtual int Price { get; set; }
        public Guid Evaluatord { get; set; }
        public Guid EvaluatedPersonId { get; set; }

        protected EvalauationBase()
        {

        }

        public EvalauationBase(Guid id, Guid evaluatord, Guid evaluatedPersonId, int speedOfCompletion, int dealing, int cleanliness, int perfection, int price)
        {

            Id = id;
            SpeedOfCompletion = speedOfCompletion;
            Dealing = dealing;
            Cleanliness = cleanliness;
            Perfection = perfection;
            Price = price;
            Evaluatord = evaluatord;
            EvaluatedPersonId = evaluatedPersonId;
        }

    }
}