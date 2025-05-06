using Imaar.UserProfiles;
using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Imaar.UserProfiles
{
    public abstract class RegisterDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string SecurityApproval { get; set; } = null!;

        public string ConcurrencyStamp { get; set; } = null!;

    }
}