using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.SecondaryAmenities
{
    public abstract class SecondaryAmenityCreateDtoBase
    {
        [Required]
        public string Name { get; set; } = null!;
        public int? Order { get; set; }
        public bool IsActive { get; set; } = true;
    }
}