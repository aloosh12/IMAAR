using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.Advertisements
{
    public abstract class GetAdvertisementsInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public string? File { get; set; }
        public DateTime? FromDateTimeMin { get; set; }
        public DateTime? FromDateTimeMax { get; set; }
        public DateTime? ToDateTimeMin { get; set; }
        public DateTime? ToDateTimeMax { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public bool? IsActive { get; set; }
        public Guid? UserProfileId { get; set; }

        public GetAdvertisementsInputBase()
        {

        }
    }
}