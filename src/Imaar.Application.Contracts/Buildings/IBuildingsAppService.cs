using Imaar.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Buildings
{
    public partial interface IBuildingsAppService : IApplicationService
    {

        Task<PagedResultDto<BuildingWithNavigationPropertiesDto>> GetListAsync(GetBuildingsInput input);

        Task<BuildingWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<BuildingDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetRegionLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetFurnishingLevelLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetBuildingFacadeLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetServiceTypeLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetMainAmenityLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetSecondaryAmenityLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<BuildingDto> CreateAsync(BuildingCreateDto input);

        Task<BuildingDto> UpdateAsync(Guid id, BuildingUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(BuildingExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> buildingIds);

        Task DeleteAllAsync(GetBuildingsInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}