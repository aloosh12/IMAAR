using Imaar.Medias;
using Imaar.ServiceTypes;
using Imaar.UserProfiles;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Imaar.ImaarServices
{
    public class ImaarServiceWithDetailsMobileDto : ImaarServiceWithNavigationPropertiesDtoBase
    {
        public List<MediaDto> Media { get; set; } = new();
    }
} 