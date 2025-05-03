using System;

namespace Imaar.Categories
{
    public abstract class CategoryExcelDtoBase
    {
        public string Title { get; set; } = null!;
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}