using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Imaar.VerificationCodes
{
    public abstract class VerificationCodeDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string PhoneNumber { get; set; } = null!;
        public int SecurityCode { get; set; }
        public bool IsFinish { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}