using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Imaar.Permissions;
using Imaar.Categories;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;
using Imaar.ServiceTypes;
using Imaar.ImaarServices;
using Imaar.Stories;
using Volo.Abp.Users;
using Imaar.Advertisements;

namespace Imaar.Pages
{

    // [Authorize(ImaarPermissions.Categories.Default)]
    [AllowAnonymous]
    public  class HomeAppService : ImaarAppService, IHomeAppService
    {
        protected ICategoriesAppService _categoriesAppService;
        protected IServiceTypesAppService _serviceTypesAppService;
        protected IStoriesAppService _storiesAppService;
        protected IImaarServicesAppService _imaarServicesAppService;
        protected IAdvertisementsAppService _advertisementsAppService;
        protected ICurrentUser _currentUser;
        public HomeAppService(ICategoriesAppService categoriesAppService, IServiceTypesAppService serviceTypesAppService, IStoriesAppService storiesAppService, IImaarServicesAppService imaarServicesAppService, IAdvertisementsAppService advertisementsAppService, ICurrentUser currentUser)
        {
            _categoriesAppService = categoriesAppService;
            _serviceTypesAppService = serviceTypesAppService;
            _storiesAppService = storiesAppService;
            _imaarServicesAppService = imaarServicesAppService;
            _advertisementsAppService = advertisementsAppService;
            _currentUser = currentUser;
        }

        public async Task<HomePageDto> GetAsync()
        {
            HomePageDto homePageDto = new HomePageDto();

            GetCategoriesInput getCategoriesInput = new GetCategoriesInput()
            {
                SkipCount = 0,
                MaxResultCount = 10
            };
            GetServiceTypesInput getServiceTypesInput = new GetServiceTypesInput()
            {
                SkipCount = 0,
                MaxResultCount = 10
            };
            GetStoriesInput getStoriesInput = new GetStoriesInput()
            {
                
                SkipCount = 0,
                MaxResultCount = 10
            };
            GetImaarServicesInput getImaarServicesInput = new GetImaarServicesInput()
            {
                SkipCount = 0,
                MaxResultCount = 10
            };
            GetAdvertisementsInput getAdvertisementsInput = new GetAdvertisementsInput()
            {
                FromDateTimeMax = DateTime.Now,
                ToDateTimeMin = DateTime.Now,
                IsActive = true,
                SkipCount = 0,
                MaxResultCount = 1000
            };
            homePageDto.CategoryDtos = (await _categoriesAppService.GetListAsync(getCategoriesInput)).Items;
            homePageDto.ServiceTypeDto = (await _serviceTypesAppService.GetListAsync(getServiceTypesInput)).Items;
            homePageDto.StoryDto = (await _storiesAppService.GetMobileListAsync(getStoriesInput)).Items;
            homePageDto.AdvertisementDtos = (await _advertisementsAppService.GetListAsync(getAdvertisementsInput)).Items;
            homePageDto.BestServiceDtos = (await _imaarServicesAppService.GetListAsync(getImaarServicesInput)).Items;
            return homePageDto;
        }
    }
}