using System;
using Volo.Abp.Application.Dtos;

namespace Imaar.ImaarServices
{
    public class ImaarServiceFilterDto : PagedAndSortedResultRequestDto
    {
        public string GeneralFilter { get; set; }
    }
} 