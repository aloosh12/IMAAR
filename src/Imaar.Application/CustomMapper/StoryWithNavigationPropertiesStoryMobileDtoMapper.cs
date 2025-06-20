﻿using Imaar.Authorizations;
using Imaar.Stories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;


namespace Imaar.CustomMapper
{
    public class StoryWithNavigationPropertiesStoryMobileDtoMapper : IObjectMapper<StoryWithNavigationProperties, StoryMobileDto>,
        IObjectMapper<List<StoryWithNavigationProperties>, List<StoryMobileDto>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;
        private readonly UserManager _userManager;
        public StoryWithNavigationPropertiesStoryMobileDtoMapper(IObjectMapper objectMapper, UserManager userManager)
        {
            _objectMapper = objectMapper;
            _userManager = userManager;
        }

        public StoryMobileDto Map(StoryWithNavigationProperties source)
        {
            StoryMobileDto storyMobileDto = new StoryMobileDto();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                storyMobileDto = new StoryMobileDto()
                {
                    Id = source.Story.Id,
                    Title = source.Story.Title,
                    FromTime = source.Story.FromTime,
                    ExpiryTime = source.Story.ExpiryTime,
                    ConcurrencyStamp = source.Story.ConcurrencyStamp,
                };
            }
            else
            {
                storyMobileDto = new StoryMobileDto()
                {
                    Id = source.Story.Id,
                    Title = source.Story.Title,
                    FromTime = source.Story.FromTime,
                    ExpiryTime = source.Story.ExpiryTime,
                    ConcurrencyStamp = source.Story.ConcurrencyStamp,
                };
            }
            IdentityUser identityUser = _userManager.FindUserByClientId(source.StoryPublisher.Id.ToString()).Result;
            if (identityUser != null)
                storyMobileDto.StoryPublisher = identityUser.Name;

            // Map media items if available
            if (source.Medias != null && source.Medias.Any())
            {
                storyMobileDto.Medias = _objectMapper.Map<List<Imaar.Medias.Media>, List<Imaar.Medias.MediaDto>>(source.Medias);
            }

            return storyMobileDto;
        }

        public StoryMobileDto Map(StoryWithNavigationProperties source, StoryMobileDto destination)
        {
            return Map(source);
        }

        public List<StoryMobileDto> Map(List<StoryWithNavigationProperties> source)
        {
            var output = new List<StoryMobileDto>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<StoryMobileDto> Map(List<StoryWithNavigationProperties> source, List<StoryMobileDto> destination)
        {
            return Map(source);
        }
    }
}

