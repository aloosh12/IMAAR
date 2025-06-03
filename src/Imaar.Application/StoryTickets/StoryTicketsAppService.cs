using Imaar.Shared;
using Imaar.Stories;
using Imaar.UserProfiles;
using Imaar.StoryTicketTypes;
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
using Imaar.StoryTickets;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.StoryTickets
{

    [Authorize(ImaarPermissions.StoryTickets.Default)]
    public abstract class StoryTicketsAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<StoryTicketDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IStoryTicketRepository _storyTicketRepository;
        protected StoryTicketManager _storyTicketManager;

        protected IRepository<Imaar.StoryTicketTypes.StoryTicketType, Guid> _storyTicketTypeRepository;
        protected IRepository<Imaar.UserProfiles.UserProfile, Guid> _userProfileRepository;
        protected IRepository<Imaar.Stories.Story, Guid> _storyRepository;

        public StoryTicketsAppServiceBase(IStoryTicketRepository storyTicketRepository, StoryTicketManager storyTicketManager, IDistributedCache<StoryTicketDownloadTokenCacheItem, string> downloadTokenCache, IRepository<Imaar.StoryTicketTypes.StoryTicketType, Guid> storyTicketTypeRepository, IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository, IRepository<Imaar.Stories.Story, Guid> storyRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _storyTicketRepository = storyTicketRepository;
            _storyTicketManager = storyTicketManager; _storyTicketTypeRepository = storyTicketTypeRepository;
            _userProfileRepository = userProfileRepository;
            _storyRepository = storyRepository;

        }

        public virtual async Task<PagedResultDto<StoryTicketWithNavigationPropertiesDto>> GetListAsync(GetStoryTicketsInput input)
        {
            var totalCount = await _storyTicketRepository.GetCountAsync(input.FilterText, input.Description, input.StoryTicketTypeId, input.TicketCreatorId, input.StoryAgainstId);
            var items = await _storyTicketRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Description, input.StoryTicketTypeId, input.TicketCreatorId, input.StoryAgainstId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<StoryTicketWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<StoryTicketWithNavigationProperties>, List<StoryTicketWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<StoryTicketWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<StoryTicketWithNavigationProperties, StoryTicketWithNavigationPropertiesDto>
                (await _storyTicketRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<StoryTicketDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<StoryTicket, StoryTicketDto>(await _storyTicketRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetStoryTicketTypeLookupAsync(LookupRequestDto input)
        {
            var query = (await _storyTicketTypeRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Title != null &&
                         x.Title.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.StoryTicketTypes.StoryTicketType>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.StoryTicketTypes.StoryTicketType>, List<LookupDto<Guid>>>(lookupData)
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

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetStoryLookupAsync(LookupRequestDto input)
        {
            var query = (await _storyRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Title != null &&
                         x.Title.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.Stories.Story>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.Stories.Story>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(ImaarPermissions.StoryTickets.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _storyTicketRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.StoryTickets.Create)]
        public virtual async Task<StoryTicketDto> CreateAsync(StoryTicketCreateDto input)
        {
            if (input.StoryTicketTypeId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["StoryTicketType"]]);
            }
            if (input.TicketCreatorId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.StoryAgainstId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Story"]]);
            }

            var storyTicket = await _storyTicketManager.CreateAsync(
            input.StoryTicketTypeId, input.TicketCreatorId, input.StoryAgainstId, input.Description
            );

            return ObjectMapper.Map<StoryTicket, StoryTicketDto>(storyTicket);
        }

        [Authorize(ImaarPermissions.StoryTickets.Edit)]
        public virtual async Task<StoryTicketDto> UpdateAsync(Guid id, StoryTicketUpdateDto input)
        {
            if (input.StoryTicketTypeId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["StoryTicketType"]]);
            }
            if (input.TicketCreatorId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.StoryAgainstId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Story"]]);
            }

            var storyTicket = await _storyTicketManager.UpdateAsync(
            id,
            input.StoryTicketTypeId, input.TicketCreatorId, input.StoryAgainstId, input.Description, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<StoryTicket, StoryTicketDto>(storyTicket);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(StoryTicketExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var storyTickets = await _storyTicketRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Description, input.StoryTicketTypeId, input.TicketCreatorId, input.StoryAgainstId);
            var items = storyTickets.Select(item => new
            {
                Description = item.StoryTicket.Description,

                StoryTicketType = item.StoryTicketType?.Title,
                TicketCreator = item.TicketCreator?.SecurityNumber,
                StoryAgainst = item.StoryAgainst?.Title,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "StoryTickets.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.StoryTickets.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> storyticketIds)
        {
            await _storyTicketRepository.DeleteManyAsync(storyticketIds);
        }

        [Authorize(ImaarPermissions.StoryTickets.Delete)]
        public virtual async Task DeleteAllAsync(GetStoryTicketsInput input)
        {
            await _storyTicketRepository.DeleteAllAsync(input.FilterText, input.Description, input.StoryTicketTypeId, input.TicketCreatorId, input.StoryAgainstId);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new StoryTicketDownloadTokenCacheItem { Token = token },
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