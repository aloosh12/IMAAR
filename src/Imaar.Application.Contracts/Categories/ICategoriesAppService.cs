using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Categories
{
    public partial interface ICategoriesAppService : IApplicationService
    {

        Task<PagedResultDto<CategoryDto>> GetListAsync(GetCategoriesInput input);

        Task<CategoryDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<CategoryDto> CreateAsync(CategoryCreateDto input);

        Task<CategoryDto> UpdateAsync(Guid id, CategoryUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(CategoryExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> categoryIds);

        Task DeleteAllAsync(GetCategoriesInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}