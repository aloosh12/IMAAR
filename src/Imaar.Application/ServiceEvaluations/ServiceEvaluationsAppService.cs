using Imaar.Shared;
using Imaar.ImaarServices;
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
using Imaar.ServiceEvaluations;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.ServiceEvaluations
{

    [Authorize(ImaarPermissions.ServiceEvaluations.Default)]
    public abstract class ServiceEvaluationsAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<ServiceEvaluationDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IServiceEvaluationRepository _serviceEvaluationRepository;
        protected ServiceEvaluationManager _serviceEvaluationManager;

        protected IRepository<Imaar.UserProfiles.UserProfile, Guid> _userProfileRepository;
        protected IRepository<Imaar.ImaarServices.ImaarService, Guid> _imaarServiceRepository;

        public ServiceEvaluationsAppServiceBase(IServiceEvaluationRepository serviceEvaluationRepository, ServiceEvaluationManager serviceEvaluationManager, IDistributedCache<ServiceEvaluationDownloadTokenCacheItem, string> downloadTokenCache, IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository, IRepository<Imaar.ImaarServices.ImaarService, Guid> imaarServiceRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _serviceEvaluationRepository = serviceEvaluationRepository;
            _serviceEvaluationManager = serviceEvaluationManager; _userProfileRepository = userProfileRepository;
            _imaarServiceRepository = imaarServiceRepository;

        }

        public virtual async Task<PagedResultDto<ServiceEvaluationWithNavigationPropertiesDto>> GetListAsync(GetServiceEvaluationsInput input)
        {
            var totalCount = await _serviceEvaluationRepository.GetCountAsync(input.FilterText, input.RateMin, input.RateMax, input.EvaluatorId, input.ImaarServiceId);
            var items = await _serviceEvaluationRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.RateMin, input.RateMax, input.EvaluatorId, input.ImaarServiceId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<ServiceEvaluationWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<ServiceEvaluationWithNavigationProperties>, List<ServiceEvaluationWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<ServiceEvaluationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<ServiceEvaluationWithNavigationProperties, ServiceEvaluationWithNavigationPropertiesDto>
                (await _serviceEvaluationRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<ServiceEvaluationDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<ServiceEvaluation, ServiceEvaluationDto>(await _serviceEvaluationRepository.GetAsync(id));
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

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetImaarServiceLookupAsync(LookupRequestDto input)
        {
            var query = (await _imaarServiceRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Title != null &&
                         x.Title.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.ImaarServices.ImaarService>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.ImaarServices.ImaarService>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(ImaarPermissions.ServiceEvaluations.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _serviceEvaluationRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.ServiceEvaluations.Create)]
        public virtual async Task<ServiceEvaluationDto> CreateAsync(ServiceEvaluationCreateDto input)
        {
            if (input.EvaluatorId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.ImaarServiceId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["ImaarService"]]);
            }

            var serviceEvaluation = await _serviceEvaluationManager.CreateAsync(
            input.EvaluatorId, input.ImaarServiceId, input.Rate
            );

            return ObjectMapper.Map<ServiceEvaluation, ServiceEvaluationDto>(serviceEvaluation);
        }

        [Authorize(ImaarPermissions.ServiceEvaluations.Edit)]
        public virtual async Task<ServiceEvaluationDto> UpdateAsync(Guid id, ServiceEvaluationUpdateDto input)
        {
            if (input.EvaluatorId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.ImaarServiceId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["ImaarService"]]);
            }

            var serviceEvaluation = await _serviceEvaluationManager.UpdateAsync(
            id,
            input.EvaluatorId, input.ImaarServiceId, input.Rate, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<ServiceEvaluation, ServiceEvaluationDto>(serviceEvaluation);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(ServiceEvaluationExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var serviceEvaluations = await _serviceEvaluationRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.RateMin, input.RateMax, input.EvaluatorId, input.ImaarServiceId);
            var items = serviceEvaluations.Select(item => new
            {
                Rate = item.ServiceEvaluation.Rate,

                Evaluator = item.Evaluator?.SecurityNumber,
                ImaarService = item.ImaarService?.Title,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "ServiceEvaluations.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.ServiceEvaluations.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> serviceevaluationIds)
        {
            await _serviceEvaluationRepository.DeleteManyAsync(serviceevaluationIds);
        }

        [Authorize(ImaarPermissions.ServiceEvaluations.Delete)]
        public virtual async Task DeleteAllAsync(GetServiceEvaluationsInput input)
        {
            await _serviceEvaluationRepository.DeleteAllAsync(input.FilterText, input.RateMin, input.RateMax, input.EvaluatorId, input.ImaarServiceId);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new ServiceEvaluationDownloadTokenCacheItem { Token = token },
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