using Imaar.Shared;
using Imaar.Vacancies;
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
using Imaar.VacancyEvaluations;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.VacancyEvaluations
{

    [Authorize(ImaarPermissions.VacancyEvaluations.Default)]
    public abstract class VacancyEvaluationsAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<VacancyEvaluationDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IVacancyEvaluationRepository _vacancyEvaluationRepository;
        protected VacancyEvaluationManager _vacancyEvaluationManager;

        protected IRepository<Imaar.UserProfiles.UserProfile, Guid> _userProfileRepository;
        protected IRepository<Imaar.Vacancies.Vacancy, Guid> _vacancyRepository;

        public VacancyEvaluationsAppServiceBase(IVacancyEvaluationRepository vacancyEvaluationRepository, VacancyEvaluationManager vacancyEvaluationManager, IDistributedCache<VacancyEvaluationDownloadTokenCacheItem, string> downloadTokenCache, IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository, IRepository<Imaar.Vacancies.Vacancy, Guid> vacancyRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _vacancyEvaluationRepository = vacancyEvaluationRepository;
            _vacancyEvaluationManager = vacancyEvaluationManager; _userProfileRepository = userProfileRepository;
            _vacancyRepository = vacancyRepository;

        }

        public virtual async Task<PagedResultDto<VacancyEvaluationWithNavigationPropertiesDto>> GetListAsync(GetVacancyEvaluationsInput input)
        {
            var totalCount = await _vacancyEvaluationRepository.GetCountAsync(input.FilterText, input.RateMin, input.RateMax, input.UserProfileId, input.VacancyId);
            var items = await _vacancyEvaluationRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.RateMin, input.RateMax, input.UserProfileId, input.VacancyId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<VacancyEvaluationWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<VacancyEvaluationWithNavigationProperties>, List<VacancyEvaluationWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<VacancyEvaluationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<VacancyEvaluationWithNavigationProperties, VacancyEvaluationWithNavigationPropertiesDto>
                (await _vacancyEvaluationRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<VacancyEvaluationDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<VacancyEvaluation, VacancyEvaluationDto>(await _vacancyEvaluationRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input)
        {
            var query = (await _userProfileRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.FirstName != null &&
                         x.FirstName.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.UserProfiles.UserProfile>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.UserProfiles.UserProfile>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetVacancyLookupAsync(LookupRequestDto input)
        {
            var query = (await _vacancyRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Title != null &&
                         x.Title.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.Vacancies.Vacancy>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.Vacancies.Vacancy>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(ImaarPermissions.VacancyEvaluations.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _vacancyEvaluationRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.VacancyEvaluations.Create)]
        public virtual async Task<VacancyEvaluationDto> CreateAsync(VacancyEvaluationCreateDto input)
        {
            if (input.UserProfileId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.VacancyId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Vacancy"]]);
            }

            var vacancyEvaluation = await _vacancyEvaluationManager.CreateAsync(
            input.UserProfileId, input.VacancyId, input.Rate
            );

            return ObjectMapper.Map<VacancyEvaluation, VacancyEvaluationDto>(vacancyEvaluation);
        }

        [Authorize(ImaarPermissions.VacancyEvaluations.Edit)]
        public virtual async Task<VacancyEvaluationDto> UpdateAsync(Guid id, VacancyEvaluationUpdateDto input)
        {
            if (input.UserProfileId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.VacancyId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Vacancy"]]);
            }

            var vacancyEvaluation = await _vacancyEvaluationManager.UpdateAsync(
            id,
            input.UserProfileId, input.VacancyId, input.Rate, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<VacancyEvaluation, VacancyEvaluationDto>(vacancyEvaluation);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(VacancyEvaluationExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var vacancyEvaluations = await _vacancyEvaluationRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.RateMin, input.RateMax, input.UserProfileId, input.VacancyId);
            var items = vacancyEvaluations.Select(item => new
            {
                Rate = item.VacancyEvaluation.Rate,

                UserProfile = item.UserProfile?.FirstName,
                Vacancy = item.Vacancy?.Title,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "VacancyEvaluations.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.VacancyEvaluations.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> vacancyevaluationIds)
        {
            await _vacancyEvaluationRepository.DeleteManyAsync(vacancyevaluationIds);
        }

        [Authorize(ImaarPermissions.VacancyEvaluations.Delete)]
        public virtual async Task DeleteAllAsync(GetVacancyEvaluationsInput input)
        {
            await _vacancyEvaluationRepository.DeleteAllAsync(input.FilterText, input.RateMin, input.RateMax, input.UserProfileId, input.VacancyId);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new VacancyEvaluationDownloadTokenCacheItem { Token = token },
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