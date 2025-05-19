using Imaar.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Evalauations
{
    public partial interface IEvalauationsAppService : IApplicationService
    {

        Task<PagedResultDto<EvalauationWithNavigationPropertiesDto>> GetListAsync(GetEvalauationsInput input);

        Task<EvalauationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<EvalauationDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<EvalauationDto> CreateAsync(EvalauationCreateDto input);

        Task<EvalauationDto> UpdateAsync(Guid id, EvalauationUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(EvalauationExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> evalauationIds);

        Task DeleteAllAsync(GetEvalauationsInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}