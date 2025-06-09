using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.ImaarServices
{
    public class GetShopListV1Input : PagedAndSortedResultRequestDto
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
        
        // Additional fields for V1
        public bool? IncludeServices { get; set; } = true;
        public bool? IncludeBuildings { get; set; } = true;
        public bool? IncludeVacancies { get; set; } = true;
        public int? RatingMin { get; set; }
        public string? Category { get; set; }
        public string? Tags { get; set; }
        
        public GetShopListV1Input()
        {
        }
    }
} 