using Imaar.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Notifications
{
    public partial interface INotificationsAppService : IApplicationService
    {

        Task<PagedResultDto<NotificationWithNavigationPropertiesDto>> GetListAsync(GetNotificationsInput input);

        Task<NotificationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<NotificationDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetNotificationTypeLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<NotificationDto> CreateAsync(NotificationCreateDto input);

        Task<NotificationDto> UpdateAsync(Guid id, NotificationUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(NotificationExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> notificationIds);

        Task DeleteAllAsync(GetNotificationsInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}