using Imaar.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Regions
{
    public partial interface IRegionsAppService : IApplicationService
    {

        Task<PagedResultDto<RegionWithNavigationPropertiesDto>> GetListAsync(GetRegionsInput input);

        Task<RegionWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<RegionDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetCityLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<RegionDto> CreateAsync(RegionCreateDto input);

        Task<RegionDto> UpdateAsync(Guid id, RegionUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(RegionExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> regionIds);

        Task DeleteAllAsync(GetRegionsInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}