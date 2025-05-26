using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Imaar.UserWorksExhibitions
{
    public  class UserWorksExhibitionMobileCreateDto
    {
        public string? Title { get; set; }
        [Required]
        public IFormFile File { get; set; }
        public int Order { get; set; }
        public Guid UserProfileId { get; set; }
    }
}