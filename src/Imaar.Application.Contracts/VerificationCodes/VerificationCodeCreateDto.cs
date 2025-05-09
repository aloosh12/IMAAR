using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.VerificationCodes
{
    public abstract class VerificationCodeCreateDtoBase
    {
        [Required]
        public string PhoneNumber { get; set; } = null!;
        public int SecurityCode { get; set; }
        public bool IsFinish { get; set; } = false;
    }
}