using Imaar.Shared;
using Imaar.UserProfiles;
using Imaar.ServiceTypes;
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
using Imaar.ImaarServices;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;
using Imaar.MobileResponses;
using Imaar.Medias;
using Imaar.Buildings;
using Imaar.Vacancies;
using Imaar.Notifications;

namespace Imaar.ImaarServices
{
    public class ImaarServicesAppService : ImaarServicesAppServiceBase, IImaarServicesAppService
    {
        protected IMediasAppService _mediasAppService;
        protected IRepository<Building, Guid> _buildingRepository;
        protected IRepository<Vacancy, Guid> _vacancyRepository;
        
        public ImaarServicesAppService(
            IImaarServiceRepository imaarServiceRepository, 
            ImaarServiceManager imaarServiceManager, 
            Volo.Abp.Caching.IDistributedCache<ImaarServiceDownloadTokenCacheItem, string> downloadTokenCache, 
            Volo.Abp.Domain.Repositories.IRepository<Imaar.ServiceTypes.ServiceType, Guid> serviceTypeRepository, 
            Volo.Abp.Domain.Repositories.IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository,
            IMediasAppService mediasAppService,
            IRepository<Building, Guid> buildingRepository,
            IRepository<Vacancy, Guid> vacancyRepository)
            : base(imaarServiceRepository, imaarServiceManager, downloadTokenCache, serviceTypeRepository, userProfileRepository)
        {
            _mediasAppService = mediasAppService;
            _buildingRepository = buildingRepository;
            _vacancyRepository = vacancyRepository;
        }

        [AllowAnonymous]
        public virtual async Task<MobileResponseDto> CreateWithFilesAsync(ImaarServiceCreateWithFilesDto input)
        {
            var result = await _imaarServiceManager.CreateWithFilesAsync(
                input.Title,
                input.Description,
                input.ServiceLocation,
                input.ServiceNumber,
                input.DateOfPublish,
                input.Price,
                input.Latitude,
                input.Longitude,
                input.ServiceTypeId,
                input.UserProfileId,
                input.Files
            );
            
            return ObjectMapper.Map<MobileResponse, MobileResponseDto>(result);
        }

        [AllowAnonymous]
        public virtual async Task<PagedResultDto<ImaarServiceShopListItemDto>> GetShopListAsync(ImaarServiceFilterDto input)
        {
            // Create a list to store the combined results
            var combinedResults = new List<ImaarServiceShopListItemDto>();

            // Search in ImaarService entities
            var imaarServices = await _imaarServiceRepository.GetListAsync(
                 !string.IsNullOrEmpty(input.GeneralFilter) ? 
                    s => s.Title.Contains(input.GeneralFilter) || s.Description.Contains(input.GeneralFilter) : 
                    null
            );

            foreach (var service in imaarServices)
            {
                // Get the first media for this service
                var media = await _mediasAppService.GetFirstMediaByEntityIdAsync(service.Id, MediaEntityType.Service);
                
                combinedResults.Add(new ImaarServiceShopListItemDto
                {
                    Id = service.Id,
                    Title = service.Title,
                    Description = service.Description,
                    Price = service.Price.ToString(),
                    MediaName = media?.File,
                    SourceEntityType = SourceEntityType.Service
                });
            }

            // Search in Building entities
            var buildings = await _buildingRepository.GetListAsync(
                !string.IsNullOrEmpty(input.GeneralFilter) ? 
                    b => b.MainTitle.Contains(input.GeneralFilter) || b.Description.Contains(input.GeneralFilter) : 
                    null
            );

            foreach (var building in buildings)
            {
                // Get the first media for this building
                var media = await _mediasAppService.GetFirstMediaByEntityIdAsync(building.Id, MediaEntityType.Building);
                
                combinedResults.Add(new ImaarServiceShopListItemDto
                {
                    Id = building.Id,
                    Title = building.MainTitle,
                    Description = building.Description,
                    Price = building.Price,
                    MediaName = media?.File,
                    SourceEntityType = SourceEntityType.Building
                });
            }

            // Search in Vacancy entities
            var vacancies = await _vacancyRepository.GetListAsync(
                 !string.IsNullOrEmpty(input.GeneralFilter) ? 
                    v => v.Title.Contains(input.GeneralFilter) || v.Description.Contains(input.GeneralFilter) : 
                    null
            );

            foreach (var vacancy in vacancies)
            {
                // Get the first media for this vacancy
                var media = await _mediasAppService.GetFirstMediaByEntityIdAsync(vacancy.Id, MediaEntityType.Vacancy);
                
                combinedResults.Add(new ImaarServiceShopListItemDto
                {
                    Id = vacancy.Id,
                    Title = vacancy.Title,
                    Description = vacancy.Description,
                    Price = vacancy.Salary,
                    MediaName = media?.File,
                    SourceEntityType = SourceEntityType.Vacancies
                });
            }

            // Apply sorting and paging
            var sortedResults = combinedResults;
            
            if (!string.IsNullOrEmpty(input.Sorting))
            {
                // Apply sorting based on input.Sorting
                if (input.Sorting.ToLower().Contains("price"))
                {
                    if (input.Sorting.ToLower().Contains("desc"))
                    {
                        sortedResults = combinedResults.OrderByDescending(x => x.Price).ToList();
                    }
                    else
                    {
                        sortedResults = combinedResults.OrderBy(x => x.Price).ToList();
                    }
                }
                else
                {
                    if (input.Sorting.ToLower().Contains("desc"))
                    {
                        sortedResults = combinedResults.OrderByDescending(x => x.Title).ToList();
                    }
                    else
                    {
                        sortedResults = combinedResults.OrderBy(x => x.Title).ToList();
                    }
                }
            }
            else
            {
                // Default sorting by Title
                sortedResults = combinedResults.OrderBy(x => x.Title).ToList();
            }
            
            // Apply paging
            var pagedResults = sortedResults
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToList();

            // Return the paged result
            return new PagedResultDto<ImaarServiceShopListItemDto>
            {
                TotalCount = combinedResults.Count,
                Items = pagedResults
            };
        }

        //Write your custom code...
    }
}