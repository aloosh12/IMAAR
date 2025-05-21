using Imaar.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.ServiceEvaluations
{
    public partial interface IServiceEvaluationsAppService : IApplicationService
    {

        Task<PagedResultDto<ServiceEvaluationWithNavigationPropertiesDto>> GetListAsync(GetServiceEvaluationsInput input);

        Task<ServiceEvaluationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<ServiceEvaluationDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetImaarServiceLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<ServiceEvaluationDto> CreateAsync(ServiceEvaluationCreateDto input);

        Task<ServiceEvaluationDto> UpdateAsync(Guid id, ServiceEvaluationUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(ServiceEvaluationExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> serviceevaluationIds);

        Task DeleteAllAsync(GetServiceEvaluationsInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}