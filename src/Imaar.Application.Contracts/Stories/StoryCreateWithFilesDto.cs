using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Imaar.Stories
{
    public class StoryCreateWithFilesDto
    {
        [Required]
        public string Title { get; set; } = null!;
        
        [Required]
        public Guid PublisherId { get; set; }
        
        [Required]
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
    }
} 