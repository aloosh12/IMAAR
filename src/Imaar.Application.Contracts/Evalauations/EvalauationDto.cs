using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Imaar.Evalauations
{
    public abstract class EvalauationDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public int SpeedOfCompletion { get; set; }
        public int Dealing { get; set; }
        public int Cleanliness { get; set; }
        public int Perfection { get; set; }
        public int Price { get; set; }
        public Guid Evaluatord { get; set; }
        public Guid EvaluatedPersonId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}