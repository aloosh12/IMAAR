using System;

namespace Imaar.Advertisements
{
    public abstract class AdvertisementExcelDtoBase
    {
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public string File { get; set; } = null!;
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}