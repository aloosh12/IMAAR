using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.VerificationCodes
{
    public abstract class GetVerificationCodesInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? PhoneNumber { get; set; }
        public int? SecurityCodeMin { get; set; }
        public int? SecurityCodeMax { get; set; }
        public bool? IsFinish { get; set; }

        public GetVerificationCodesInputBase()
        {

        }
    }
}