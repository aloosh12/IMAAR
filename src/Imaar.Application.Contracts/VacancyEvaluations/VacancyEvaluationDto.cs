using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Imaar.VacancyEvaluations
{
    public abstract class VacancyEvaluationDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public int Rate { get; set; }
        public Guid UserProfileId { get; set; }
        public Guid VacancyId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}