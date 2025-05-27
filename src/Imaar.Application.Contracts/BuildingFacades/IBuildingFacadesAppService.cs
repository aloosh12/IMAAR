using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.BuildingFacades
{
    public partial interface IBuildingFacadesAppService : IApplicationService
    {

        Task<PagedResultDto<BuildingFacadeDto>> GetListAsync(GetBuildingFacadesInput input);

        Task<BuildingFacadeDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<BuildingFacadeDto> CreateAsync(BuildingFacadeCreateDto input);

        Task<BuildingFacadeDto> UpdateAsync(Guid id, BuildingFacadeUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(BuildingFacadeExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> buildingfacadeIds);

        Task DeleteAllAsync(GetBuildingFacadesInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}