using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.VacancyAdditionalFeatures
{
    public partial interface IVacancyAdditionalFeaturesAppService : IApplicationService
    {

        Task<PagedResultDto<VacancyAdditionalFeatureDto>> GetListAsync(GetVacancyAdditionalFeaturesInput input);

        Task<VacancyAdditionalFeatureDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<VacancyAdditionalFeatureDto> CreateAsync(VacancyAdditionalFeatureCreateDto input);

        Task<VacancyAdditionalFeatureDto> UpdateAsync(Guid id, VacancyAdditionalFeatureUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(VacancyAdditionalFeatureExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> vacancyadditionalfeatureIds);

        Task DeleteAllAsync(GetVacancyAdditionalFeaturesInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}