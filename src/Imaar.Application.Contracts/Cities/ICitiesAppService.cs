using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Cities
{
    public partial interface ICitiesAppService : IApplicationService
    {

        Task<PagedResultDto<CityDto>> GetListAsync(GetCitiesInput input);

        Task<CityDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<CityDto> CreateAsync(CityCreateDto input);

        Task<CityDto> UpdateAsync(Guid id, CityUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(CityExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> cityIds);

        Task DeleteAllAsync(GetCitiesInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}