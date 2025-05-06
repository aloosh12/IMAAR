using Imaar.UserProfiles;
using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Imaar.UserProfiles
{
    public abstract class UserProfileDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string SecurityNumber { get; set; } = null!;
        public BiologicalSex? BiologicalSex { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? ProfilePhoto { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}