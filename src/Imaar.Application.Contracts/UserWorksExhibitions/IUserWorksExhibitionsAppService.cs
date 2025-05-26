using Imaar.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.UserWorksExhibitions
{
    public partial interface IUserWorksExhibitionsAppService : IApplicationService
    {

        Task<PagedResultDto<UserWorksExhibitionWithNavigationPropertiesDto>> GetListAsync(GetUserWorksExhibitionsInput input);

        Task<UserWorksExhibitionWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<UserWorksExhibitionDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<UserWorksExhibitionDto> CreateAsync(UserWorksExhibitionCreateDto input);

        Task<UserWorksExhibitionDto> UpdateAsync(Guid id, UserWorksExhibitionUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(UserWorksExhibitionExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> userworksexhibitionIds);

        Task DeleteAllAsync(GetUserWorksExhibitionsInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}