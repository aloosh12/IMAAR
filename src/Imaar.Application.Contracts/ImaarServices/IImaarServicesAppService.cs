using Imaar.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.ImaarServices
{
    public partial interface IImaarServicesAppService : IApplicationService
    {

        Task<PagedResultDto<ImaarServiceWithNavigationPropertiesDto>> GetListAsync(GetImaarServicesInput input);

        Task<ImaarServiceWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<ImaarServiceDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetServiceTypeLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<ImaarServiceDto> CreateAsync(ImaarServiceCreateDto input);

        Task<ImaarServiceDto> UpdateAsync(Guid id, ImaarServiceUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(ImaarServiceExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> imaarserviceIds);

        Task DeleteAllAsync(GetImaarServicesInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}