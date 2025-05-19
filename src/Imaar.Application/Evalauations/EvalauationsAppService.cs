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
using Imaar.Evalauations;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.Evalauations
{

    [Authorize(ImaarPermissions.Evalauations.Default)]
    public abstract class EvalauationsAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<EvalauationDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IEvalauationRepository _evalauationRepository;
        protected EvalauationManager _evalauationManager;

        protected IRepository<Imaar.UserProfiles.UserProfile, Guid> _userProfileRepository;

        public EvalauationsAppServiceBase(IEvalauationRepository evalauationRepository, EvalauationManager evalauationManager, IDistributedCache<EvalauationDownloadTokenCacheItem, string> downloadTokenCache, IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _evalauationRepository = evalauationRepository;
            _evalauationManager = evalauationManager; _userProfileRepository = userProfileRepository;

        }

        public virtual async Task<PagedResultDto<EvalauationWithNavigationPropertiesDto>> GetListAsync(GetEvalauationsInput input)
        {
            var totalCount = await _evalauationRepository.GetCountAsync(input.FilterText, input.SpeedOfCompletionMin, input.SpeedOfCompletionMax, input.DealingMin, input.DealingMax, input.CleanlinessMin, input.CleanlinessMax, input.PerfectionMin, input.PerfectionMax, input.PriceMin, input.PriceMax, input.Evaluatord, input.EvaluatedPersonId);
            var items = await _evalauationRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.SpeedOfCompletionMin, input.SpeedOfCompletionMax, input.DealingMin, input.DealingMax, input.CleanlinessMin, input.CleanlinessMax, input.PerfectionMin, input.PerfectionMax, input.PriceMin, input.PriceMax, input.Evaluatord, input.EvaluatedPersonId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<EvalauationWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<EvalauationWithNavigationProperties>, List<EvalauationWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<EvalauationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<EvalauationWithNavigationProperties, EvalauationWithNavigationPropertiesDto>
                (await _evalauationRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<EvalauationDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Evalauation, EvalauationDto>(await _evalauationRepository.GetAsync(id));
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

        [Authorize(ImaarPermissions.Evalauations.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _evalauationRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.Evalauations.Create)]
        public virtual async Task<EvalauationDto> CreateAsync(EvalauationCreateDto input)
        {
            if (input.Evaluatord == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.EvaluatedPersonId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }

            var evalauation = await _evalauationManager.CreateAsync(
            input.Evaluatord, input.EvaluatedPersonId, input.SpeedOfCompletion, input.Dealing, input.Cleanliness, input.Perfection, input.Price
            );

            return ObjectMapper.Map<Evalauation, EvalauationDto>(evalauation);
        }

        [Authorize(ImaarPermissions.Evalauations.Edit)]
        public virtual async Task<EvalauationDto> UpdateAsync(Guid id, EvalauationUpdateDto input)
        {
            if (input.Evaluatord == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.EvaluatedPersonId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }

            var evalauation = await _evalauationManager.UpdateAsync(
            id,
            input.Evaluatord, input.EvaluatedPersonId, input.SpeedOfCompletion, input.Dealing, input.Cleanliness, input.Perfection, input.Price, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Evalauation, EvalauationDto>(evalauation);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(EvalauationExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var evalauations = await _evalauationRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.SpeedOfCompletionMin, input.SpeedOfCompletionMax, input.DealingMin, input.DealingMax, input.CleanlinessMin, input.CleanlinessMax, input.PerfectionMin, input.PerfectionMax, input.PriceMin, input.PriceMax, input.Evaluatord, input.EvaluatedPersonId);
            var items = evalauations.Select(item => new
            {
                SpeedOfCompletion = item.Evalauation.SpeedOfCompletion,
                Dealing = item.Evalauation.Dealing,
                Cleanliness = item.Evalauation.Cleanliness,
                Perfection = item.Evalauation.Perfection,
                Price = item.Evalauation.Price,

                Evaluatord = item.Evaluatord?.SecurityNumber,
                EvaluatedPerson = item.EvaluatedPerson?.SecurityNumber,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Evalauations.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.Evalauations.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> evalauationIds)
        {
            await _evalauationRepository.DeleteManyAsync(evalauationIds);
        }

        [Authorize(ImaarPermissions.Evalauations.Delete)]
        public virtual async Task DeleteAllAsync(GetEvalauationsInput input)
        {
            await _evalauationRepository.DeleteAllAsync(input.FilterText, input.SpeedOfCompletionMin, input.SpeedOfCompletionMax, input.DealingMin, input.DealingMax, input.CleanlinessMin, input.CleanlinessMax, input.PerfectionMin, input.PerfectionMax, input.PriceMin, input.PriceMax, input.Evaluatord, input.EvaluatedPersonId);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new EvalauationDownloadTokenCacheItem { Token = token },
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