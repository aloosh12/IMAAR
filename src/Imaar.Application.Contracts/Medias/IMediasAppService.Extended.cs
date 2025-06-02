using System;
using System.Threading.Tasks;
using Imaar.Notifications;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Imaar.Medias
{
    public partial interface IMediasAppService
    {
        //Write your custom code here...
        Task<MediaDto> GetFirstMediaByEntityIdAsync(Guid entityId, MediaEntityType sourceEntityType);
        Task<List<MediaDto>> BulkUpdateMediasAsync(MediaBulkUpdateDto input);
    }
}