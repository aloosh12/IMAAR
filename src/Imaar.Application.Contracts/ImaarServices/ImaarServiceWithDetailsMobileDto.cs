using Imaar.Medias;
using Imaar.ServiceTypes;
using Imaar.UserProfiles;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Imaar.ImaarServices
{
    public class ImaarServiceWithDetailsMobileDto 
    {
        public ImaarServiceDto ImaarService { get; set; } = null!;
        public ServiceTypeDto ServiceType { get; set; } = null!;
        public UserProfileWithDetailsDto UserProfileWithDetailsDto { get; set; } = null!;
        public List<MediaDto> Media { get; set; } = new();
    }
} 