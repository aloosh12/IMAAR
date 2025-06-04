using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.ImaarServices
{
    public abstract class GetImaarServicesInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ServiceLocation { get; set; }
        public string? ServiceNumber { get; set; }
        public DateOnly? DateOfPublishMin { get; set; }
        public DateOnly? DateOfPublishMax { get; set; }
        public int? PriceMin { get; set; }
        public int? PriceMax { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public int? ViewCounterMin { get; set; }
        public int? ViewCounterMax { get; set; }
        public int? OrderCounterMin { get; set; }
        public int? OrderCounterMax { get; set; }
        public Guid? ServiceTypeId { get; set; }
        public Guid? UserProfileId { get; set; }

        public GetImaarServicesInputBase()
        {

        }
    }
}