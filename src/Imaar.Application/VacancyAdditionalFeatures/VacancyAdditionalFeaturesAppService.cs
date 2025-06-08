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
using Imaar.VacancyAdditionalFeatures;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.VacancyAdditionalFeatures
{

    [Authorize(ImaarPermissions.VacancyAdditionalFeatures.Default)]
    public abstract class VacancyAdditionalFeaturesAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<VacancyAdditionalFeatureDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IVacancyAdditionalFeatureRepository _vacancyAdditionalFeatureRepository;
        protected VacancyAdditionalFeatureManager _vacancyAdditionalFeatureManager;

        public VacancyAdditionalFeaturesAppServiceBase(IVacancyAdditionalFeatureRepository vacancyAdditionalFeatureRepository, VacancyAdditionalFeatureManager vacancyAdditionalFeatureManager, IDistributedCache<VacancyAdditionalFeatureDownloadTokenCacheItem, string> downloadTokenCache)
        {
            _downloadTokenCache = downloadTokenCache;
            _vacancyAdditionalFeatureRepository = vacancyAdditionalFeatureRepository;
            _vacancyAdditionalFeatureManager = vacancyAdditionalFeatureManager;

        }

        public virtual async Task<PagedResultDto<VacancyAdditionalFeatureDto>> GetListAsync(GetVacancyAdditionalFeaturesInput input)
        {
            var totalCount = await _vacancyAdditionalFeatureRepository.GetCountAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive);
            var items = await _vacancyAdditionalFeatureRepository.GetListAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<VacancyAdditionalFeatureDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<VacancyAdditionalFeature>, List<VacancyAdditionalFeatureDto>>(items)
            };
        }

        public virtual async Task<VacancyAdditionalFeatureDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<VacancyAdditionalFeature, VacancyAdditionalFeatureDto>(await _vacancyAdditionalFeatureRepository.GetAsync(id));
        }

        [Authorize(ImaarPermissions.VacancyAdditionalFeatures.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _vacancyAdditionalFeatureRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.VacancyAdditionalFeatures.Create)]
        public virtual async Task<VacancyAdditionalFeatureDto> CreateAsync(VacancyAdditionalFeatureCreateDto input)
        {

            var vacancyAdditionalFeature = await _vacancyAdditionalFeatureManager.CreateAsync(
            input.Name, input.Order, input.IsActive
            );

            return ObjectMapper.Map<VacancyAdditionalFeature, VacancyAdditionalFeatureDto>(vacancyAdditionalFeature);
        }

        [Authorize(ImaarPermissions.VacancyAdditionalFeatures.Edit)]
        public virtual async Task<VacancyAdditionalFeatureDto> UpdateAsync(Guid id, VacancyAdditionalFeatureUpdateDto input)
        {

            var vacancyAdditionalFeature = await _vacancyAdditionalFeatureManager.UpdateAsync(
            id,
            input.Name, input.Order, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<VacancyAdditionalFeature, VacancyAdditionalFeatureDto>(vacancyAdditionalFeature);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(VacancyAdditionalFeatureExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _vacancyAdditionalFeatureRepository.GetListAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<VacancyAdditionalFeature>, List<VacancyAdditionalFeatureExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "VacancyAdditionalFeatures.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.VacancyAdditionalFeatures.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> vacancyadditionalfeatureIds)
        {
            await _vacancyAdditionalFeatureRepository.DeleteManyAsync(vacancyadditionalfeatureIds);
        }

        [Authorize(ImaarPermissions.VacancyAdditionalFeatures.Delete)]
        public virtual async Task DeleteAllAsync(GetVacancyAdditionalFeaturesInput input)
        {
            await _vacancyAdditionalFeatureRepository.DeleteAllAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new VacancyAdditionalFeatureDownloadTokenCacheItem { Token = token },
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