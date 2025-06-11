using Imaar.Shared;
using Imaar.Buildings;
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
using Imaar.BuildingEvaluations;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.BuildingEvaluations
{

    [Authorize(ImaarPermissions.BuildingEvaluations.Default)]
    public abstract class BuildingEvaluationsAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<BuildingEvaluationDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IBuildingEvaluationRepository _buildingEvaluationRepository;
        protected BuildingEvaluationManager _buildingEvaluationManager;

        protected IRepository<Imaar.UserProfiles.UserProfile, Guid> _userProfileRepository;
        protected IRepository<Imaar.Buildings.Building, Guid> _buildingRepository;

        public BuildingEvaluationsAppServiceBase(IBuildingEvaluationRepository buildingEvaluationRepository, BuildingEvaluationManager buildingEvaluationManager, IDistributedCache<BuildingEvaluationDownloadTokenCacheItem, string> downloadTokenCache, IRepository<Imaar.UserProfiles.UserProfile, Guid> userProfileRepository, IRepository<Imaar.Buildings.Building, Guid> buildingRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _buildingEvaluationRepository = buildingEvaluationRepository;
            _buildingEvaluationManager = buildingEvaluationManager; _userProfileRepository = userProfileRepository;
            _buildingRepository = buildingRepository;

        }

        public virtual async Task<PagedResultDto<BuildingEvaluationWithNavigationPropertiesDto>> GetListAsync(GetBuildingEvaluationsInput input)
        {
            var totalCount = await _buildingEvaluationRepository.GetCountAsync(input.FilterText, input.RateMin, input.RateMax, input.EvaluatorId, input.BuildingId);
            var items = await _buildingEvaluationRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.RateMin, input.RateMax, input.EvaluatorId, input.BuildingId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<BuildingEvaluationWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<BuildingEvaluationWithNavigationProperties>, List<BuildingEvaluationWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<BuildingEvaluationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<BuildingEvaluationWithNavigationProperties, BuildingEvaluationWithNavigationPropertiesDto>
                (await _buildingEvaluationRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<BuildingEvaluationDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<BuildingEvaluation, BuildingEvaluationDto>(await _buildingEvaluationRepository.GetAsync(id));
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

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetBuildingLookupAsync(LookupRequestDto input)
        {
            var query = (await _buildingRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.MainTitle != null &&
                         x.MainTitle.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.Buildings.Building>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.Buildings.Building>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(ImaarPermissions.BuildingEvaluations.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _buildingEvaluationRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.BuildingEvaluations.Create)]
        public virtual async Task<BuildingEvaluationDto> CreateAsync(BuildingEvaluationCreateDto input)
        {
            if (input.EvaluatorId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.BuildingId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Building"]]);
            }

            var buildingEvaluation = await _buildingEvaluationManager.CreateAsync(
            input.EvaluatorId, input.BuildingId, input.Rate
            );

            return ObjectMapper.Map<BuildingEvaluation, BuildingEvaluationDto>(buildingEvaluation);
        }

        [Authorize(ImaarPermissions.BuildingEvaluations.Edit)]
        public virtual async Task<BuildingEvaluationDto> UpdateAsync(Guid id, BuildingEvaluationUpdateDto input)
        {
            if (input.EvaluatorId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["UserProfile"]]);
            }
            if (input.BuildingId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Building"]]);
            }

            var buildingEvaluation = await _buildingEvaluationManager.UpdateAsync(
            id,
            input.EvaluatorId, input.BuildingId, input.Rate, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<BuildingEvaluation, BuildingEvaluationDto>(buildingEvaluation);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(BuildingEvaluationExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var buildingEvaluations = await _buildingEvaluationRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.RateMin, input.RateMax, input.EvaluatorId, input.BuildingId);
            var items = buildingEvaluations.Select(item => new
            {
                Rate = item.BuildingEvaluation.Rate,

                Evaluator = item.Evaluator?.FirstName,
                Building = item.Building?.MainTitle,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "BuildingEvaluations.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.BuildingEvaluations.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> buildingevaluationIds)
        {
            await _buildingEvaluationRepository.DeleteManyAsync(buildingevaluationIds);
        }

        [Authorize(ImaarPermissions.BuildingEvaluations.Delete)]
        public virtual async Task DeleteAllAsync(GetBuildingEvaluationsInput input)
        {
            await _buildingEvaluationRepository.DeleteAllAsync(input.FilterText, input.RateMin, input.RateMax, input.EvaluatorId, input.BuildingId);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new BuildingEvaluationDownloadTokenCacheItem { Token = token },
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