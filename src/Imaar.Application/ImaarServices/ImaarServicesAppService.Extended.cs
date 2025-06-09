using Imaar.Buildings;
using Imaar.ImaarServices;
using Imaar.Medias;
using Imaar.MobileResponses;
using Imaar.Notifications;
using Imaar.Permissions;
using Imaar.ServiceEvaluations;
using Imaar.ServiceTypes;
using Imaar.Shared;
using Imaar.Shared;
using Imaar.UserProfiles;
using Imaar.Vacancies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using Polly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;

namespace Imaar.ImaarServices
{
    public class ImaarServicesAppService : ImaarServicesAppServiceBase, IImaarServicesAppService
    {
        private readonly IMediasAppService _mediasAppService;
        private readonly IBuildingRepository _buildingRepository;
        private readonly IVacancyRepository _vacancyRepository;
        private readonly IServiceEvaluationsAppService _serviceEvaluationsAppService;

        public ImaarServicesAppService(
            IImaarServiceRepository imaarServiceRepository,
            ImaarServiceManager imaarServiceManager,
            IDistributedCache<ImaarServiceDownloadTokenCacheItem, string> downloadTokenCache,
            IRepository<ServiceType, Guid> serviceTypeRepository,
            IUserProfileRepository userProfileRepository,
            IMediasAppService mediasAppService,
            IBuildingRepository buildingRepository,
            IVacancyRepository vacancyRepository,
            IServiceEvaluationsAppService serviceEvaluationsAppService)
            : base(imaarServiceRepository, imaarServiceManager, downloadTokenCache, serviceTypeRepository, userProfileRepository)
        {
            _mediasAppService = mediasAppService;
            _buildingRepository = buildingRepository;
            _vacancyRepository = vacancyRepository;
            _serviceEvaluationsAppService = serviceEvaluationsAppService;
        }

        [AllowAnonymous]
        public virtual async Task<long> GetServiceNumber()
        {
          long serviceCount =await  _imaarServiceRepository.GetCountAsync();
            //long buildingCount = await _buildingRepository.GetCountAsync();
            //long vacancyCount = await _vacancyRepository.GetCountAsync();
            long buildingCount = 0;
            long vacancyCount = 0;
        return serviceCount + buildingCount + vacancyCount;
        }
        [AllowAnonymous]
        public virtual async Task<MobileResponseDto> CreateWithFilesAsync(ImaarServiceCreateWithFilesDto input)
        {
            var serviceNumber = await GetServiceNumber();
            var result = await _imaarServiceManager.CreateWithFilesAsync(
                input.Title,
                input.Description,
                input.ServiceLocation,
                serviceNumber.ToString(),
                DateOnly.FromDateTime(DateTime.Now),
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
        public virtual async Task<MobileResponseDto> GetImaarServiceWithDetailsAsync(Guid id)
        {
            var mobileResponse = new MobileResponse();
            try
            {
                // Get the ImaarService with navigation properties
                var imaarServiceWithDetails = await GetWithNavigationPropertiesAsync(id);
                
                if (imaarServiceWithDetails == null || imaarServiceWithDetails.ImaarService == null)
                {
                    mobileResponse.Code = 404;
                    mobileResponse.Message = "ImaarService not found";
                    mobileResponse.Data = null;
                    
                    return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
                }
                
                // Get media for this imaar service
                var getMediasInput = new GetMediasInput
                {
                    SkipCount = 0,
                    MaxResultCount = 1000,
                    SourceEntityId = id.ToString(),
                    SourceEntityType = MediaEntityType.Service,
                    IsActive = true,
                    Sorting = "Order asc"
                };
                var mediaListDto = await _mediasAppService.GetListAsync(getMediasInput);
                var usertemp = ObjectMapper.Map<UserProfileWithDetails, UserProfileWithDetailsDto>(await _userProfileRepository.GetWithDetailsAsync(imaarServiceWithDetails.UserProfile.Id));
                // Create result DTO
                var result = new ImaarServiceWithDetailsMobileDto
                {
                    ImaarService = imaarServiceWithDetails.ImaarService,
                    ServiceType = imaarServiceWithDetails.ServiceType,
                    UserProfileWithDetailsDto = usertemp,
                    Media = mediaListDto != null ? mediaListDto.Items.ToList() : new List<MediaDto>()
                };
                
                result.ImaarService.ServiceEval = await _serviceEvaluationsAppService.GetAverageEvaluationForServiceAsync(id);
                // Increment the view counter asynchronously (fire and forget)
                _ = IncrementViewCounterAsync(id);

               

                mobileResponse.Code = 200;
                mobileResponse.Message = "ImaarService retrieved successfully";
                mobileResponse.Data = result;
                
                return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
            }
            catch (Exception ex)
            {
                mobileResponse.Code = 500;
                mobileResponse.Message = ex.Message;
                mobileResponse.Data = null;
                
                return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
            }
        }

        [AllowAnonymous]
        public virtual async Task<PagedResultDto<ImaarServiceShopListItemDto>> GetShopListAsync(GetShopListInput input)
        {
            // Create a list to store the combined results
            var combinedResults = new List<ImaarServiceShopListItemDto>();

            GetImaarServicesInput getImaarServicesInput = new GetImaarServicesInput();

            // Search in ImaarService entities
            var imaarServices = await _imaarServiceRepository.GetListAsync(
                 !string.IsNullOrEmpty(input.FilterText) ? 
                    s => s.Title.Contains(input.FilterText) || s.Description.Contains(input.FilterText) : 
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
                    MediaName = $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/MediaImages/{media?.File}",
                    SourceEntityType = SourceEntityType.Service
                });
            }

            // Search in Building entities
            var buildings = await _buildingRepository.GetListAsync(
                !string.IsNullOrEmpty(input.FilterText) ? 
                    b => b.MainTitle.Contains(input.FilterText) || b.Description.Contains(input.FilterText) : 
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
                 !string.IsNullOrEmpty(input.FilterText) ? 
                    v => v.Title.Contains(input.FilterText) || v.Description.Contains(input.FilterText) : 
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

        [AllowAnonymous]
        public virtual async Task<PagedResultDto<ImaarServiceShopListItemDto>> GetShopListV1Async(GetShopListV1Input input)
        {
            // Create a list to store the combined results
            var combinedResults = new List<ImaarServiceShopListItemDto>();

            // Search in ImaarService entities
            if (input.IncludeServices != false)
            {
                var imaarServicesQuery = await _imaarServiceRepository.GetQueryableAsync();
                
                // Apply filter criteria
                if (!string.IsNullOrEmpty(input.FilterText))
                {
                    imaarServicesQuery = imaarServicesQuery.Where(s => 
                        s.Title.Contains(input.FilterText) || 
                        s.Description.Contains(input.FilterText));
                }
                
                if (!string.IsNullOrEmpty(input.ServiceLocation))
                {
                    imaarServicesQuery = imaarServicesQuery.Where(s => 
                        s.ServiceLocation.Contains(input.ServiceLocation));
                }
                
                if (input.DateOfPublishMin.HasValue)
                {
                    imaarServicesQuery = imaarServicesQuery.Where(s => 
                        s.DateOfPublish >= input.DateOfPublishMin.Value);
                }
                
                if (input.DateOfPublishMax.HasValue)
                {
                    imaarServicesQuery = imaarServicesQuery.Where(s => 
                        s.DateOfPublish <= input.DateOfPublishMax.Value);
                }
                
                if (input.PriceMin.HasValue)
                {
                    imaarServicesQuery = imaarServicesQuery.Where(s => 
                        s.Price >= input.PriceMin.Value);
                }
                
                if (input.PriceMax.HasValue)
                {
                    imaarServicesQuery = imaarServicesQuery.Where(s => 
                        s.Price <= input.PriceMax.Value);
                }
                
                if (input.ServiceTypeId.HasValue)
                {
                    imaarServicesQuery = imaarServicesQuery.Where(s => 
                        s.ServiceTypeId == input.ServiceTypeId.Value);
                }
                
                if (input.UserProfileId.HasValue)
                {
                    imaarServicesQuery = imaarServicesQuery.Where(s => 
                        s.UserProfileId == input.UserProfileId.Value);
                }
                
                // If category is specified, get service types for this category and filter
                if (!string.IsNullOrEmpty(input.Category))
                {
                    var serviceTypes = await _serviceTypeRepository.GetListAsync(st => 
                        st.Title.Contains(input.Category));
                    
                    if (serviceTypes.Any())
                    {
                        var serviceTypeIds = serviceTypes.Select(st => st.Id).ToList();
                        imaarServicesQuery = imaarServicesQuery.Where(s => 
                            serviceTypeIds.Contains(s.ServiceTypeId));
                    }
                }
                
                // Execute query
                var imaarServices = await AsyncExecuter.ToListAsync(imaarServicesQuery);
                
                // Apply additional filters that might require service evaluations
                List<ImaarService> filteredServices = new List<ImaarService>();
                
                if (input.RatingMin.HasValue)
                {
                    foreach (var service in imaarServices)
                    {
                        var avgRating = await _serviceEvaluationsAppService.GetAverageEvaluationForServiceAsync(service.Id);
                        if (avgRating >= input.RatingMin.Value)
                        {
                            filteredServices.Add(service);
                        }
                    }
                }
                else
                {
                    filteredServices = imaarServices;
                }
                
                // Map to DTOs
                foreach (var service in filteredServices)
                {
                    // Get the first media for this service
                    var media = await _mediasAppService.GetFirstMediaByEntityIdAsync(service.Id, MediaEntityType.Service);
                    
                    combinedResults.Add(new ImaarServiceShopListItemDto
                    {
                        Id = service.Id,
                        Title = service.Title,
                        Description = service.Description,
                        Price = service.Price.ToString(),
                        MediaName = $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/MediaImages/{media?.File}",
                        SourceEntityType = SourceEntityType.Service
                    });
                }
            }

            // Search in Building entities
            if (input.IncludeBuildings != false)
            {
                var buildingsQuery = await _buildingRepository.GetQueryableAsync();
                
                if (!string.IsNullOrEmpty(input.FilterText))
                {
                    buildingsQuery = buildingsQuery.Where(b => 
                        b.MainTitle.Contains(input.FilterText) || 
                        b.Description.Contains(input.FilterText));
                }
                
                //if (!string.IsNullOrEmpty(input.ServiceLocation))
                //{
                //    buildingsQuery = buildingsQuery.Where(b => 
                //        b.Address.Contains(input.ServiceLocation));
                //}
                
                if (input.PriceMin.HasValue)
                {
                    buildingsQuery = buildingsQuery.Where(b => 
                        decimal.Parse(b.Price) >= input.PriceMin.Value);
                }
                
                if (input.PriceMax.HasValue)
                {
                    buildingsQuery = buildingsQuery.Where(b => 
                        decimal.Parse(b.Price) <= input.PriceMax.Value);
                }
                
                // Execute query
                var buildings = await AsyncExecuter.ToListAsync(buildingsQuery);
                
                // Map to DTOs
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
                        MediaName = $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/MediaImages/{media?.File}",
                        SourceEntityType = SourceEntityType.Building
                    });
                }
            }

            // Search in Vacancy entities
            if (input.IncludeVacancies != false)
            {
                var vacanciesQuery = await _vacancyRepository.GetQueryableAsync();
                
                if (!string.IsNullOrEmpty(input.FilterText))
                {
                    vacanciesQuery = vacanciesQuery.Where(v => 
                        v.Title.Contains(input.FilterText) || 
                        v.Description.Contains(input.FilterText));
                }
                
                if (!string.IsNullOrEmpty(input.ServiceLocation))
                {
                    vacanciesQuery = vacanciesQuery.Where(v => 
                        v.Location.Contains(input.ServiceLocation));
                }
                
                if (input.PriceMin.HasValue)
                {
                    vacanciesQuery = vacanciesQuery.Where(v => 
                        !v.Salary.IsNullOrEmpty() && decimal.Parse(v.Salary) >= input.PriceMin.Value);
                }
                
                if (input.PriceMax.HasValue)
                {
                    vacanciesQuery = vacanciesQuery.Where(v =>
                        !v.Salary.IsNullOrEmpty() && decimal.Parse(v.Salary) <= input.PriceMax.Value);
                }
                
                // Execute query
                var vacancies = await AsyncExecuter.ToListAsync(vacanciesQuery);
                
                // Map to DTOs
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
                        MediaName = $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/MediaImages/{media?.File}",
                        SourceEntityType = SourceEntityType.Vacancies
                    });
                }
            }
            
            // Filter by tags if provided
            if (!string.IsNullOrEmpty(input.Tags))
            {
                var tags = input.Tags.Split(',').Select(t => t.Trim().ToLower()).ToList();
                combinedResults = combinedResults
                    .Where(r => tags.Any(tag => 
                        r.Title.ToLower().Contains(tag) || 
                        r.Description.ToLower().Contains(tag)))
                    .ToList();
            }

            // Apply sorting
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

        [AllowAnonymous]
        public virtual async Task<MobileResponseDto> IncrementViewCounterAsync(Guid id)
        {
            var mobileResponse = new MobileResponse();
            
            try
            {
                // Get the ImaarService
                var imaarService = await _imaarServiceRepository.GetAsync(id);
                
                if (imaarService == null)
                {
                    mobileResponse.Code = 404;
                    mobileResponse.Message = "ImaarService not found";
                    mobileResponse.Data = null;
                    
                    return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
                }
                
                // Increment the view counter
                imaarService.ViewCounter += 1;
                
                // Update the entity
                await _imaarServiceRepository.UpdateAsync(imaarService);
                
                mobileResponse.Code = 200;
                mobileResponse.Message = "View counter incremented successfully";
                mobileResponse.Data = new { 
                    ImaarServiceId = imaarService.Id, 
                    ViewCounter = imaarService.ViewCounter 
                };
                
                return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
            }
            catch (Exception ex)
            {
                mobileResponse.Code = 500;
                mobileResponse.Message = ex.Message;
                mobileResponse.Data = null;
                
                return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
            }
        }
        
        [AllowAnonymous]
        public virtual async Task<MobileResponseDto> IncrementOrderCounterAsync(Guid id)
        {
            var mobileResponse = new MobileResponse();
            
            try
            {
                // Get the ImaarService
                var imaarService = await _imaarServiceRepository.GetAsync(id);
                
                if (imaarService == null)
                {
                    mobileResponse.Code = 404;
                    mobileResponse.Message = "ImaarService not found";
                    mobileResponse.Data = null;
                    
                    return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
                }
                
                // Increment the order counter
                imaarService.OrderCounter += 1;
                
                // Update the entity
                await _imaarServiceRepository.UpdateAsync(imaarService);
                
                mobileResponse.Code = 200;
                mobileResponse.Message = "Order counter incremented successfully";
                mobileResponse.Data = new { 
                    ImaarServiceId = imaarService.Id, 
                    OrderCounter = imaarService.OrderCounter 
                };
                
                return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
            }
            catch (Exception ex)
            {
                mobileResponse.Code = 500;
                mobileResponse.Message = ex.Message;
                mobileResponse.Data = null;
                
                return ObjectMapper.Map<MobileResponse, MobileResponseDto>(mobileResponse);
            }
        }
    }
}