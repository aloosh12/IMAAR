using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.Categories;
using Volo.Abp.Content;
using Imaar.Shared;
using Microsoft.AspNetCore.Authorization;

namespace Imaar.Controllers.Categories
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Category")]
    [Route("api/app/categories")]

    public abstract class CategoryControllerBase : AbpController
    {
        protected ICategoriesAppService _categoriesAppService;

        public CategoryControllerBase(ICategoriesAppService categoriesAppService)
        {
            _categoriesAppService = categoriesAppService;
        }
        [AllowAnonymous]
        [HttpGet]
        public virtual Task<PagedResultDto<CategoryDto>> GetListAsync(GetCategoriesInput input)
        {
            return _categoriesAppService.GetListAsync(input);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("{id}")]
        public virtual Task<CategoryDto> GetAsync(Guid id)
        {
            return _categoriesAppService.GetAsync(id);
        }
        [AllowAnonymous]
        [HttpPost]
        public virtual Task<CategoryDto> CreateAsync(CategoryCreateDto input)
        {
            return _categoriesAppService.CreateAsync(input);
        }

        [AllowAnonymous]
        [HttpPut]
        [Route("{id}")]
        public virtual Task<CategoryDto> UpdateAsync(Guid id, CategoryUpdateDto input)
        {
            return _categoriesAppService.UpdateAsync(id, input);
        }
        [AllowAnonymous]
        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _categoriesAppService.DeleteAsync(id);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("as-excel-file-cate")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(CategoryExcelDownloadDto input)
        {
            return _categoriesAppService.GetListAsExcelFileAsync(input);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _categoriesAppService.GetDownloadTokenAsync();
        }
        [AllowAnonymous]
        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> categoryIds)
        {
            return _categoriesAppService.DeleteByIdsAsync(categoryIds);
        }
        [AllowAnonymous]
        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetCategoriesInput input)
        {
            return _categoriesAppService.DeleteAllAsync(input);
        }
    }
}