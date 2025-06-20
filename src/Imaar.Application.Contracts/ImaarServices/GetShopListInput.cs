using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.ImaarServices
{
    public abstract class GetShopListInput : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }
        public string? ServiceLocation { get; set; }
        public DateOnly? DateOfPublishMin { get; set; }
        public DateOnly? DateOfPublishMax { get; set; }
        public int? PriceMin { get; set; }
        public int? PriceMax { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public Guid? ServiceTypeId { get; set; }
        public Guid? UserProfileId { get; set; }
        public GetShopListInput()
        {

        }
    }
}