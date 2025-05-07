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
using Imaar.Categories;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.Categories
{

    // [Authorize(ImaarPermissions.Categories.Default)]
    [AllowAnonymous]
    public abstract class CategoriesAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<CategoryDownloadTokenCacheItem, string> _downloadTokenCache;
        protected ICategoryRepository _categoryRepository;
        protected CategoryManager _categoryManager;

        public CategoriesAppServiceBase(ICategoryRepository categoryRepository, CategoryManager categoryManager, IDistributedCache<CategoryDownloadTokenCacheItem, string> downloadTokenCache)
        {
            _downloadTokenCache = downloadTokenCache;
            _categoryRepository = categoryRepository;
            _categoryManager = categoryManager;

        }

        public virtual async Task<PagedResultDto<CategoryDto>> GetListAsync(GetCategoriesInput input)
        {
            var totalCount = await _categoryRepository.GetCountAsync(input.FilterText, input.Title, input.OrderMin, input.OrderMax, input.IsActive);
            var items = await _categoryRepository.GetListAsync(input.FilterText, input.Title, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<CategoryDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Category>, List<CategoryDto>>(items)
            };
        }

        public virtual async Task<CategoryDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Category, CategoryDto>(await _categoryRepository.GetAsync(id));
        }

        // [Authorize(ImaarPermissions.Categories.Delete)]
        [AllowAnonymous]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _categoryRepository.DeleteAsync(id);
        }

        //[Authorize(ImaarPermissions.Categories.Create)]
        [AllowAnonymous]
        public virtual async Task<CategoryDto> CreateAsync(CategoryCreateDto input)
        {

            var category = await _categoryManager.CreateAsync(
            input.Title, input.Icon, input.Order, input.IsActive
            );

            return ObjectMapper.Map<Category, CategoryDto>(category);
        }

        //[Authorize(ImaarPermissions.Categories.Edit)]
        [AllowAnonymous]
        public virtual async Task<CategoryDto> UpdateAsync(Guid id, CategoryUpdateDto input)
        {

            var category = await _categoryManager.UpdateAsync(
            id,
            input.Title, input.Icon, input.Order, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Category, CategoryDto>(category);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(CategoryExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _categoryRepository.GetListAsync(input.FilterText, input.Title, input.OrderMin, input.OrderMax, input.IsActive);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<Category>, List<CategoryExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Categories.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        // [Authorize(ImaarPermissions.Categories.Delete)]
        [AllowAnonymous]
        public virtual async Task DeleteByIdsAsync(List<Guid> categoryIds)
        {
            await _categoryRepository.DeleteManyAsync(categoryIds);
        }

        // [Authorize(ImaarPermissions.Categories.Delete)]
        [AllowAnonymous]
        public virtual async Task DeleteAllAsync(GetCategoriesInput input)
        {
            await _categoryRepository.DeleteAllAsync(input.FilterText, input.Title, input.OrderMin, input.OrderMax, input.IsActive);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new CategoryDownloadTokenCacheItem { Token = token },
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