using Imaar.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.VacancyEvaluations
{
    public partial interface IVacancyEvaluationsAppService : IApplicationService
    {

        Task<PagedResultDto<VacancyEvaluationWithNavigationPropertiesDto>> GetListAsync(GetVacancyEvaluationsInput input);

        Task<VacancyEvaluationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<VacancyEvaluationDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetVacancyLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<VacancyEvaluationDto> CreateAsync(VacancyEvaluationCreateDto input);

        Task<VacancyEvaluationDto> UpdateAsync(Guid id, VacancyEvaluationUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(VacancyEvaluationExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> vacancyevaluationIds);

        Task DeleteAllAsync(GetVacancyEvaluationsInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}