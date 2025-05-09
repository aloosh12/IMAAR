using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.ServiceTypes
{
    public abstract class ServiceTypeUpdateDtoBase
    {
        public Guid CategoryId { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        public string? Icon { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }

    }
}