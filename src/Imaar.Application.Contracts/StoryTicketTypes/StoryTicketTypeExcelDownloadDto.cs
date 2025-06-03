using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.StoryTicketTypes
{
    public abstract class StoryTicketTypeExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public string? Title { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public bool? IsActive { get; set; }

        public StoryTicketTypeExcelDownloadDtoBase()
        {

        }
    }
}