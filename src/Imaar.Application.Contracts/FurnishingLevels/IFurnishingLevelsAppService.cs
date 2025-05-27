using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.FurnishingLevels
{
    public partial interface IFurnishingLevelsAppService : IApplicationService
    {

        Task<PagedResultDto<FurnishingLevelDto>> GetListAsync(GetFurnishingLevelsInput input);

        Task<FurnishingLevelDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<FurnishingLevelDto> CreateAsync(FurnishingLevelCreateDto input);

        Task<FurnishingLevelDto> UpdateAsync(Guid id, FurnishingLevelUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(FurnishingLevelExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> furnishinglevelIds);

        Task DeleteAllAsync(GetFurnishingLevelsInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}