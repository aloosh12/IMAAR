using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.NotificationTypes
{
    public partial interface INotificationTypesAppService : IApplicationService
    {

        Task<PagedResultDto<NotificationTypeDto>> GetListAsync(GetNotificationTypesInput input);

        Task<NotificationTypeDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<NotificationTypeDto> CreateAsync(NotificationTypeCreateDto input);

        Task<NotificationTypeDto> UpdateAsync(Guid id, NotificationTypeUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(NotificationTypeExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> notificationtypeIds);

        Task DeleteAllAsync(GetNotificationTypesInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}