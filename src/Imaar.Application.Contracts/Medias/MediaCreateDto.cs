using Imaar.Medias;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.Medias
{
    public abstract class MediaCreateDtoBase
    {
        public string? Title { get; set; }
        [Required]
        public string File { get; set; } = null!;
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;
        public MediaEntityType SourceEntityType { get; set; } = ((MediaEntityType[])Enum.GetValues(typeof(MediaEntityType)))[0];
        [Required]
        public string SourceEntityId { get; set; } = null!;
    }
}