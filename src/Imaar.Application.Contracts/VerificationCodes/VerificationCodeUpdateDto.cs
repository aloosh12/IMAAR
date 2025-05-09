using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Imaar.VerificationCodes
{
    public abstract class VerificationCodeUpdateDtoBase : IHasConcurrencyStamp
    {
        [Required]
        public string PhoneNumber { get; set; } = null!;
        public int SecurityCode { get; set; }
        public bool IsFinish { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}