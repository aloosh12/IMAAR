using Imaar.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.UserSavedItems
{
    public partial interface IUserSavedItemsAppService : IApplicationService
    {

        Task<PagedResultDto<UserSavedItemWithNavigationPropertiesDto>> GetListAsync(GetUserSavedItemsInput input);

        Task<UserSavedItemWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<UserSavedItemDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<UserSavedItemDto> CreateAsync(UserSavedItemCreateDto input);

        Task<UserSavedItemDto> UpdateAsync(Guid id, UserSavedItemUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(UserSavedItemExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> usersaveditemIds);

        Task DeleteAllAsync(GetUserSavedItemsInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}