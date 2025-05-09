using System;

namespace Imaar.VerificationCodes
{
    public abstract class VerificationCodeExcelDtoBase
    {
        public string PhoneNumber { get; set; } = null!;
        public int SecurityCode { get; set; }
        public bool IsFinish { get; set; }
    }
}