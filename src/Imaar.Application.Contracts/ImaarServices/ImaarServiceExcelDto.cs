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
    }
}