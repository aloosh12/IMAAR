using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.Categories
{
    public abstract class GetCategoriesInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Title { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public bool? IsActive { get; set; }

        public GetCategoriesInputBase()
        {

        }
    }
}