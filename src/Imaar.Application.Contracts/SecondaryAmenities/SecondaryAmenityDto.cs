using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Imaar.SecondaryAmenities
{
    public abstract class SecondaryAmenityDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Name { get; set; } = null!;
        public int? Order { get; set; }
        public bool IsActive { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}