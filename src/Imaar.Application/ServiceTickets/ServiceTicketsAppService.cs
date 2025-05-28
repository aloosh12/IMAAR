using Imaar.Shared;
using Imaar.UserProfiles;
using Imaar.ServiceTicketTypes;
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
using Imaar.ServiceTickets;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.ServiceTickets
{

    [Authorize(ImaarPermissions.ServiceTickets.Default)]
    public abstract class ServiceTicketsAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<ServiceTicketDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IServiceTicketRepository _serviceTicketRepository;
        protected ServiceTicketManager _serviceTicketManager;

        protected IRepository<Imaar.ServiceTicketTypes.ServiceTicketType, Guid> _serviceTicketTypeRepository;
        protected IRepository<Imaar.UserProfiles.UserProfile, Guid> _userProfileRepository;

        public ServiceTicketsAppServiceBase(IServiceTicketRepository serviceTicketRepository, ServiceTicketManager serviceTicketManager, IDistributedCache<ServiceTicketDownloadTokenCacheItem, string> downloadTokenCache, IRepository<Imaar.ServiceTicketTypes.ServiceTicketType, Guid> serviceTicketTypeRepository, IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _serviceTicketRepository = serviceTicketRepository;
            _serviceTicketManager = serviceTicketManager; _serviceTicketTypeRepository = serviceTicketTypeRepository;
            _userProfileRepository = userProfileRepository;

        }

        public virtual async Task<PagedResultDto<ServiceTicketWithNavigationPropertiesDto>> GetListAsync(GetServiceTicketsInput input)
        {
            var totalCount = await _serviceTicketRepository.GetCountAsync(input.FilterText, input.Description, input.TicketEntityType, input.TicketEntityId, input.ServiceTicketTypeId, input.TicketCreatorId);
            var items = await _serviceTicketRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Description, input.TicketEntityType, input.TicketEntityId, input.ServiceTicketTypeId, input.TicketCreatorId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<ServiceTicketWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<ServiceTicketWithNavigationProperties>, List<ServiceTicketWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<ServiceTicketWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<ServiceTicketWithNavigationProperties, ServiceTicketWithNavigationPropertiesDto>
                (await _serviceTicketRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<ServiceTicketDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<ServiceTicket, ServiceTicketDto>(await _serviceTicketRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetServiceTicketTypeLookupAsync(LookupRequestDto input)
        {
            var query = (await _serviceTicketTypeRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Title != null &&
                         x.Title.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.ServiceTicketTypes.ServiceTicketType>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.ServiceTicketTypes.ServiceTicketType>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input)
        {
            var query = (await _userProfileRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.SecurityNumber != null &&
                         x.SecurityNumber.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.UserProfiles.UserProfile>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.UserProfiles.UserProfile>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(ImaarPermissions.ServiceTickets.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _serviceTicketRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.ServiceTickets.Create)]
        public virtual async Task<ServiceTicketDto> CreateAsync(ServiceTicketCreateDto input)
        {
            if (input.ServiceTicketTypeId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["ServiceTicketType"]]);
            }
            if (input.TicketCreatorId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }

            var serviceTicket = await _serviceTicketManager.CreateAsync(
            input.ServiceTicketTypeId, input.TicketCreatorId, input.Description, input.TicketEntityType, input.TicketEntityId
            );

            return ObjectMapper.Map<ServiceTicket, ServiceTicketDto>(serviceTicket);
        }

        [Authorize(ImaarPermissions.ServiceTickets.Edit)]
        public virtual async Task<ServiceTicketDto> UpdateAsync(Guid id, ServiceTicketUpdateDto input)
        {
            if (input.ServiceTicketTypeId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["ServiceTicketType"]]);
            }
            if (input.TicketCreatorId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }

            var serviceTicket = await _serviceTicketManager.UpdateAsync(
            id,
            input.ServiceTicketTypeId, input.TicketCreatorId, input.Description, input.TicketEntityType, input.TicketEntityId, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<ServiceTicket, ServiceTicketDto>(serviceTicket);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(ServiceTicketExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var serviceTickets = await _serviceTicketRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Description, input.TicketEntityType, input.TicketEntityId, input.ServiceTicketTypeId, input.TicketCreatorId);
            var items = serviceTickets.Select(item => new
            {
                Description = item.ServiceTicket.Description,
                TicketEntityType = item.ServiceTicket.TicketEntityType,
                TicketEntityId = item.ServiceTicket.TicketEntityId,

                ServiceTicketType = item.ServiceTicketType?.Title,
                TicketCreator = item.TicketCreator?.SecurityNumber,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "ServiceTickets.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.ServiceTickets.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> serviceticketIds)
        {
            await _serviceTicketRepository.DeleteManyAsync(serviceticketIds);
        }

        [Authorize(ImaarPermissions.ServiceTickets.Delete)]
        public virtual async Task DeleteAllAsync(GetServiceTicketsInput input)
        {
            await _serviceTicketRepository.DeleteAllAsync(input.FilterText, input.Description, input.TicketEntityType, input.TicketEntityId, input.ServiceTicketTypeId, input.TicketCreatorId);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new ServiceTicketDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new Imaar.Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}