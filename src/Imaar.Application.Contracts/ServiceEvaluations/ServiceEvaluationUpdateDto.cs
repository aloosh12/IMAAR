using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Imaar.ServiceEvaluations
{
    public abstract class ServiceEvaluationUpdateDtoBase : IHasConcurrencyStamp
    {
        public int Rate { get; set; }
        public Guid EvaluatorId { get; set; }
        public Guid ImaarServiceId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}