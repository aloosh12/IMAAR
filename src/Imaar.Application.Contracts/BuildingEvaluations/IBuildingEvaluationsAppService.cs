using Imaar.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.BuildingEvaluations
{
    public partial interface IBuildingEvaluationsAppService : IApplicationService
    {

        Task<PagedResultDto<BuildingEvaluationWithNavigationPropertiesDto>> GetListAsync(GetBuildingEvaluationsInput input);

        Task<BuildingEvaluationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<BuildingEvaluationDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetBuildingLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<BuildingEvaluationDto> CreateAsync(BuildingEvaluationCreateDto input);

        Task<BuildingEvaluationDto> UpdateAsync(Guid id, BuildingEvaluationUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(BuildingEvaluationExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> buildingevaluationIds);

        Task DeleteAllAsync(GetBuildingEvaluationsInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}