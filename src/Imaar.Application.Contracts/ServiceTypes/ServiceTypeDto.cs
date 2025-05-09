using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;

namespace Imaar.ServiceTypes
{
    public abstract class ServiceTypeDtoBase : FullAuditedEntityDto<Guid>
    {
        public Guid CategoryId { get; set; }
        public string Title { get; set; } = null!;
        public string? Icon { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }

    }
}