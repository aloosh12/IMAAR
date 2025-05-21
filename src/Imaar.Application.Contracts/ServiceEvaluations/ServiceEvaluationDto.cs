using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Imaar.ServiceEvaluations
{
    public abstract class ServiceEvaluationDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public int Rate { get; set; }
        public Guid EvaluatorId { get; set; }
        public Guid ImaarServiceId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}