using System;

namespace Imaar.ImaarServices
{
    public abstract class ImaarServiceExcelDtoBase
    {
        public string Title { get; set; } = null!;
        public string ServiceLocation { get; set; } = null!;
        public string ServiceNumber { get; set; } = null!;
        public DateOnly DateOfPublish { get; set; }
        public int Price { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public int ViewCounter { get; set; }
        public int OrderCounter { get; set; }
    }
}