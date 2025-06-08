using Imaar.Advertisements;
using Imaar.Categories;
using Imaar.ImaarServices;
using Imaar.ServiceTypes;
using Imaar.Stories;
using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Imaar.Pages
{
    public  class HomePageDto
    {
        public IReadOnlyList<CategoryDto> CategoryDtos { get; set; }
        public IReadOnlyList<ServiceTypeDto> ServiceTypeDto { get; set; }
        public IReadOnlyList<StoryMobileDto> StoryDto { get; set; }
        public IReadOnlyList<AdvertisementWithNavigationPropertiesDto> AdvertisementDtos { get; set; }
        public IReadOnlyList<ImaarServiceWithNavigationPropertiesDto> BestServiceDtos { get; set; }

    }
}