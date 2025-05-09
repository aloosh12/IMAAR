using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.ServiceTypes
{
    public abstract class GetServiceTypesInputBase : PagedAndSortedResultRequestDto
    {
        public Guid? CategoryId { get; set; }

        public string? FilterText { get; set; }

        public string? Title { get; set; }
        public string? Icon { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public bool? IsActive { get; set; }

        public GetServiceTypesInputBase()
        {

        }
    }
}