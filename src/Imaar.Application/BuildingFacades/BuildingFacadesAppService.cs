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
using Imaar.BuildingFacades;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.BuildingFacades
{

    [Authorize(ImaarPermissions.BuildingFacades.Default)]
    public abstract class BuildingFacadesAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<BuildingFacadeDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IBuildingFacadeRepository _buildingFacadeRepository;
        protected BuildingFacadeManager _buildingFacadeManager;

        public BuildingFacadesAppServiceBase(IBuildingFacadeRepository buildingFacadeRepository, BuildingFacadeManager buildingFacadeManager, IDistributedCache<BuildingFacadeDownloadTokenCacheItem, string> downloadTokenCache)
        {
            _downloadTokenCache = downloadTokenCache;
            _buildingFacadeRepository = buildingFacadeRepository;
            _buildingFacadeManager = buildingFacadeManager;

        }

        public virtual async Task<PagedResultDto<BuildingFacadeDto>> GetListAsync(GetBuildingFacadesInput input)
        {
            var totalCount = await _buildingFacadeRepository.GetCountAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive);
            var items = await _buildingFacadeRepository.GetListAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<BuildingFacadeDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<BuildingFacade>, List<BuildingFacadeDto>>(items)
            };
        }

        public virtual async Task<BuildingFacadeDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<BuildingFacade, BuildingFacadeDto>(await _buildingFacadeRepository.GetAsync(id));
        }

        [Authorize(ImaarPermissions.BuildingFacades.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _buildingFacadeRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.BuildingFacades.Create)]
        public virtual async Task<BuildingFacadeDto> CreateAsync(BuildingFacadeCreateDto input)
        {

            var buildingFacade = await _buildingFacadeManager.CreateAsync(
            input.Name, input.IsActive, input.Order
            );

            return ObjectMapper.Map<BuildingFacade, BuildingFacadeDto>(buildingFacade);
        }

        [Authorize(ImaarPermissions.BuildingFacades.Edit)]
        public virtual async Task<BuildingFacadeDto> UpdateAsync(Guid id, BuildingFacadeUpdateDto input)
        {

            var buildingFacade = await _buildingFacadeManager.UpdateAsync(
            id,
            input.Name, input.IsActive, input.Order, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<BuildingFacade, BuildingFacadeDto>(buildingFacade);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(BuildingFacadeExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _buildingFacadeRepository.GetListAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<BuildingFacade>, List<BuildingFacadeExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "BuildingFacades.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.BuildingFacades.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> buildingfacadeIds)
        {
            await _buildingFacadeRepository.DeleteManyAsync(buildingfacadeIds);
        }

        [Authorize(ImaarPermissions.BuildingFacades.Delete)]
        public virtual async Task DeleteAllAsync(GetBuildingFacadesInput input)
        {
            await _buildingFacadeRepository.DeleteAllAsync(input.FilterText, input.Name, input.OrderMin, input.OrderMax, input.IsActive);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new BuildingFacadeDownloadTokenCacheItem { Token = token },
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