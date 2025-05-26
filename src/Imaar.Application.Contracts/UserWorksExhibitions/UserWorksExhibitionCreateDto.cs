using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.UserWorksExhibitions
{
    public abstract class UserWorksExhibitionCreateDtoBase
    {
        public string? Title { get; set; }
        [Required]
        public string File { get; set; } = null!;
        public int Order { get; set; }
        public Guid UserProfileId { get; set; }
    }
}