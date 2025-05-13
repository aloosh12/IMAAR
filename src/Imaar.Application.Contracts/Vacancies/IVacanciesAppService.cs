using Imaar.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Vacancies
{
    public partial interface IVacanciesAppService : IApplicationService
    {

        Task<PagedResultDto<VacancyWithNavigationPropertiesDto>> GetListAsync(GetVacanciesInput input);

        Task<VacancyWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<VacancyDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetServiceTypeLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<VacancyDto> CreateAsync(VacancyCreateDto input);

        Task<VacancyDto> UpdateAsync(Guid id, VacancyUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(VacancyExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> vacancyIds);

        Task DeleteAllAsync(GetVacanciesInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}