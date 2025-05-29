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

namespace Imaar.ImaarServices
{
    public class ImaarServicesAppService : ImaarServicesAppServiceBase, IImaarServicesAppService
    {
        protected IMediasAppService _mediasAppService;
        
        public ImaarServicesAppService(
            IImaarServiceRepository imaarServiceRepository, 
            ImaarServiceManager imaarServiceManager, 
            Volo.Abp.Caching.IDistributedCache<ImaarServiceDownloadTokenCacheItem, string> downloadTokenCache, 
            Volo.Abp.Domain.Repositories.IRepository<Imaar.ServiceTypes.ServiceType, Guid> serviceTypeRepository, 
            Volo.Abp.Domain.Repositories.IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository,
            IMediasAppService mediasAppService)
            : base(imaarServiceRepository, imaarServiceManager, downloadTokenCache, serviceTypeRepository, userProfileRepository)
        {
            _mediasAppService = mediasAppService;
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

        //Write your custom code...
    }
}