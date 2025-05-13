using Imaar.Shared;
using Imaar.UserProfiles;
using Imaar.TicketTypes;
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
using Imaar.Tickets;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.Tickets
{

    [Authorize(ImaarPermissions.Tickets.Default)]
    public abstract class TicketsAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<TicketDownloadTokenCacheItem, string> _downloadTokenCache;
        protected ITicketRepository _ticketRepository;
        protected TicketManager _ticketManager;

        protected IRepository<Imaar.TicketTypes.TicketType, Guid> _ticketTypeRepository;
        protected IRepository<Imaar.UserProfiles.UserProfile, Guid> _userProfileRepository;

        public TicketsAppServiceBase(ITicketRepository ticketRepository, TicketManager ticketManager, IDistributedCache<TicketDownloadTokenCacheItem, string> downloadTokenCache, IRepository<Imaar.TicketTypes.TicketType, Guid> ticketTypeRepository, IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _ticketRepository = ticketRepository;
            _ticketManager = ticketManager; _ticketTypeRepository = ticketTypeRepository;
            _userProfileRepository = userProfileRepository;

        }

        public virtual async Task<PagedResultDto<TicketWithNavigationPropertiesDto>> GetListAsync(GetTicketsInput input)
        {
            var totalCount = await _ticketRepository.GetCountAsync(input.FilterText, input.Description, input.TicketTypeId, input.TicketCreatorId, input.TicketAgainstId);
            var items = await _ticketRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Description, input.TicketTypeId, input.TicketCreatorId, input.TicketAgainstId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<TicketWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<TicketWithNavigationProperties>, List<TicketWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<TicketWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<TicketWithNavigationProperties, TicketWithNavigationPropertiesDto>
                (await _ticketRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<TicketDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Ticket, TicketDto>(await _ticketRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetTicketTypeLookupAsync(LookupRequestDto input)
        {
            var query = (await _ticketTypeRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Title != null &&
                         x.Title.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.TicketTypes.TicketType>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.TicketTypes.TicketType>, List<LookupDto<Guid>>>(lookupData)
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

        [Authorize(ImaarPermissions.Tickets.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _ticketRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.Tickets.Create)]
        public virtual async Task<TicketDto> CreateAsync(TicketCreateDto input)
        {
            if (input.TicketTypeId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["TicketType"]]);
            }
            if (input.TicketCreatorId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.TicketAgainstId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }

            var ticket = await _ticketManager.CreateAsync(
            input.TicketTypeId, input.TicketCreatorId, input.TicketAgainstId, input.Description
            );

            return ObjectMapper.Map<Ticket, TicketDto>(ticket);
        }

        [Authorize(ImaarPermissions.Tickets.Edit)]
        public virtual async Task<TicketDto> UpdateAsync(Guid id, TicketUpdateDto input)
        {
            if (input.TicketTypeId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["TicketType"]]);
            }
            if (input.TicketCreatorId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.TicketAgainstId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }

            var ticket = await _ticketManager.UpdateAsync(
            id,
            input.TicketTypeId, input.TicketCreatorId, input.TicketAgainstId, input.Description, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Ticket, TicketDto>(ticket);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(TicketExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var tickets = await _ticketRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Description, input.TicketTypeId, input.TicketCreatorId, input.TicketAgainstId);
            var items = tickets.Select(item => new
            {
                Description = item.Ticket.Description,

                TicketType = item.TicketType?.Title,
                TicketCreator = item.TicketCreator?.SecurityNumber,
                TicketAgainst = item.TicketAgainst?.SecurityNumber,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Tickets.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.Tickets.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> ticketIds)
        {
            await _ticketRepository.DeleteManyAsync(ticketIds);
        }

        [Authorize(ImaarPermissions.Tickets.Delete)]
        public virtual async Task DeleteAllAsync(GetTicketsInput input)
        {
            await _ticketRepository.DeleteAllAsync(input.FilterText, input.Description, input.TicketTypeId, input.TicketCreatorId, input.TicketAgainstId);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new TicketDownloadTokenCacheItem { Token = token },
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