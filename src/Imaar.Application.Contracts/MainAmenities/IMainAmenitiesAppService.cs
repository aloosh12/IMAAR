using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.MainAmenities
{
    public partial interface IMainAmenitiesAppService : IApplicationService
    {

        Task<PagedResultDto<MainAmenityDto>> GetListAsync(GetMainAmenitiesInput input);

        Task<MainAmenityDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<MainAmenityDto> CreateAsync(MainAmenityCreateDto input);

        Task<MainAmenityDto> UpdateAsync(Guid id, MainAmenityUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(MainAmenityExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> mainamenityIds);

        Task DeleteAllAsync(GetMainAmenitiesInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}