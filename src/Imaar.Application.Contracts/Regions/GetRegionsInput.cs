using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.Regions
{
    public abstract class GetRegionsInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Name { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public bool? IsActive { get; set; }
        public Guid? CityId { get; set; }

        public GetRegionsInputBase()
        {

        }
    }
}