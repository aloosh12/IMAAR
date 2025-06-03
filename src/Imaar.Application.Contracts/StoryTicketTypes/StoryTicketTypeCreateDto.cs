using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.StoryTicketTypes
{
    public abstract class StoryTicketTypeCreateDtoBase
    {
        [Required]
        public string Title { get; set; } = null!;
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;
    }
}