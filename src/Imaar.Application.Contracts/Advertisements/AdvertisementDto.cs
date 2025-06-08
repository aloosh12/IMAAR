using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Imaar.Advertisements
{
    public abstract class AdvertisementDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public string File { get; set; } = null!;
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public Guid? UserProfileId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}