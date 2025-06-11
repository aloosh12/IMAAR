using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Imaar.BuildingEvaluations
{
    public abstract class BuildingEvaluationDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public int Rate { get; set; }
        public Guid EvaluatorId { get; set; }
        public Guid BuildingId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}