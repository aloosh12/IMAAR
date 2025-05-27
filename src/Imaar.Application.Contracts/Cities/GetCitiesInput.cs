using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.Cities
{
    public abstract class GetCitiesInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Name { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public bool? IsActive { get; set; }

        public GetCitiesInputBase()
        {

        }
    }
}