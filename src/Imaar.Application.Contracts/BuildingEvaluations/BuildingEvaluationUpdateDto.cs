using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Imaar.BuildingEvaluations
{
    public abstract class BuildingEvaluationUpdateDtoBase : IHasConcurrencyStamp
    {
        public int Rate { get; set; }
        public Guid EvaluatorId { get; set; }
        public Guid BuildingId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}