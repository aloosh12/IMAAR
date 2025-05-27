using Imaar.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.UserFollows
{
    public partial interface IUserFollowsAppService : IApplicationService
    {

        Task<PagedResultDto<UserFollowWithNavigationPropertiesDto>> GetListAsync(GetUserFollowsInput input);

        Task<UserFollowWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<UserFollowDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<UserFollowDto> CreateAsync(UserFollowCreateDto input);

        Task<UserFollowDto> UpdateAsync(Guid id, UserFollowUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(UserFollowExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> userfollowIds);

        Task DeleteAllAsync(GetUserFollowsInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}