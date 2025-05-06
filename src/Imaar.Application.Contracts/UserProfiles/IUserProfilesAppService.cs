using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.UserProfiles
{
    public partial interface IUserProfilesAppService : IApplicationService
    {

        Task<PagedResultDto<UserProfileDto>> GetListAsync(GetUserProfilesInput input);

        Task<UserProfileDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<UserProfileDto> CreateAsync(UserProfileCreateDto input);

        Task<UserProfileDto> UpdateAsync(Guid id, UserProfileUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(UserProfileExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> userprofileIds);

        Task DeleteAllAsync(GetUserProfilesInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}