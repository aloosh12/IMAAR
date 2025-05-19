using Imaar.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.UserEvalauations
{
    public partial interface IUserEvalauationsAppService : IApplicationService
    {

        Task<PagedResultDto<UserEvalauationWithNavigationPropertiesDto>> GetListAsync(GetUserEvalauationsInput input);

        Task<UserEvalauationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<UserEvalauationDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<UserEvalauationDto> CreateAsync(UserEvalauationCreateDto input);

        Task<UserEvalauationDto> UpdateAsync(Guid id, UserEvalauationUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(UserEvalauationExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> userEvalauationIds);

        Task DeleteAllAsync(GetUserEvalauationsInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}