using Imaar.Shared;
using Imaar.UserProfiles;
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
using Imaar.UserEvalauations;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.UserEvalauations
{

    [Authorize(ImaarPermissions.UserEvalauations.Default)]
    public abstract class UserEvalauationsAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<UserEvalauationDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IUserEvalauationRepository _userEvalauationRepository;
        protected UserEvalauationManager _userEvalauationManager;

        protected IRepository<Imaar.UserProfiles.UserProfile, Guid> _userProfileRepository;

        public UserEvalauationsAppServiceBase(IUserEvalauationRepository userEvalauationRepository, UserEvalauationManager userEvalauationManager, IDistributedCache<UserEvalauationDownloadTokenCacheItem, string> downloadTokenCache, IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _userEvalauationRepository = userEvalauationRepository;
            _userEvalauationManager = userEvalauationManager; _userProfileRepository = userProfileRepository;

        }

        public virtual async Task<PagedResultDto<UserEvalauationWithNavigationPropertiesDto>> GetListAsync(GetUserEvalauationsInput input)
        {
            var totalCount = await _userEvalauationRepository.GetCountAsync(input.FilterText, input.SpeedOfCompletionMin, input.SpeedOfCompletionMax, input.DealingMin, input.DealingMax, input.CleanlinessMin, input.CleanlinessMax, input.PerfectionMin, input.PerfectionMax, input.PriceMin, input.PriceMax, input.Evaluatord, input.EvaluatedPersonId);
            var items = await _userEvalauationRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.SpeedOfCompletionMin, input.SpeedOfCompletionMax, input.DealingMin, input.DealingMax, input.CleanlinessMin, input.CleanlinessMax, input.PerfectionMin, input.PerfectionMax, input.PriceMin, input.PriceMax, input.Evaluatord, input.EvaluatedPersonId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<UserEvalauationWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<UserEvalauationWithNavigationProperties>, List<UserEvalauationWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<UserEvalauationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<UserEvalauationWithNavigationProperties, UserEvalauationWithNavigationPropertiesDto>
                (await _userEvalauationRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<UserEvalauationDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<UserEvalauation, UserEvalauationDto>(await _userEvalauationRepository.GetAsync(id));
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

        [Authorize(ImaarPermissions.UserEvalauations.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _userEvalauationRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.UserEvalauations.Create)]
        public virtual async Task<UserEvalauationDto> CreateAsync(UserEvalauationCreateDto input)
        {
            if (input.Evaluatord == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.EvaluatedPersonId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }

            var userEvalauation = await _userEvalauationManager.CreateAsync(
            input.Evaluatord, input.EvaluatedPersonId, input.SpeedOfCompletion, input.Dealing, input.Cleanliness, input.Perfection, input.Price
            );

            return ObjectMapper.Map<UserEvalauation, UserEvalauationDto>(userEvalauation);
        }

        [Authorize(ImaarPermissions.UserEvalauations.Edit)]
        public virtual async Task<UserEvalauationDto> UpdateAsync(Guid id, UserEvalauationUpdateDto input)
        {
            if (input.Evaluatord == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.EvaluatedPersonId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }

            var userEvalauation = await _userEvalauationManager.UpdateAsync(
            id,
            input.Evaluatord, input.EvaluatedPersonId, input.SpeedOfCompletion, input.Dealing, input.Cleanliness, input.Perfection, input.Price, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<UserEvalauation, UserEvalauationDto>(userEvalauation);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(UserEvalauationExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var userEvalauations = await _userEvalauationRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.SpeedOfCompletionMin, input.SpeedOfCompletionMax, input.DealingMin, input.DealingMax, input.CleanlinessMin, input.CleanlinessMax, input.PerfectionMin, input.PerfectionMax, input.PriceMin, input.PriceMax, input.Evaluatord, input.EvaluatedPersonId);
            var items = userEvalauations.Select(item => new
            {
                SpeedOfCompletion = item.UserEvalauation.SpeedOfCompletion,
                Dealing = item.UserEvalauation.Dealing,
                Cleanliness = item.UserEvalauation.Cleanliness,
                Perfection = item.UserEvalauation.Perfection,
                Price = item.UserEvalauation.Price,

                Evaluatord = item.Evaluatord?.SecurityNumber,
                EvaluatedPerson = item.EvaluatedPerson?.SecurityNumber,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "UserEvalauations.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.UserEvalauations.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> userEvalauationIds)
        {
            await _userEvalauationRepository.DeleteManyAsync(userEvalauationIds);
        }

        [Authorize(ImaarPermissions.UserEvalauations.Delete)]
        public virtual async Task DeleteAllAsync(GetUserEvalauationsInput input)
        {
            await _userEvalauationRepository.DeleteAllAsync(input.FilterText, input.SpeedOfCompletionMin, input.SpeedOfCompletionMax, input.DealingMin, input.DealingMax, input.CleanlinessMin, input.CleanlinessMax, input.PerfectionMin, input.PerfectionMax, input.PriceMin, input.PriceMax, input.Evaluatord, input.EvaluatedPersonId);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new UserEvalauationDownloadTokenCacheItem { Token = token },
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