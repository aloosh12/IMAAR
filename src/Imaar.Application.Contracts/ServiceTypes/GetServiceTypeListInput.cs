using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.ServiceTypes
{
    public class GetServiceTypeListInput : PagedAndSortedResultRequestDto
    {
        public Guid CategoryId { get; set; }
    }
}