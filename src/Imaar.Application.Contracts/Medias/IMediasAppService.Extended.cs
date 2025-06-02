using System;
using System.Threading.Tasks;
using Imaar.Notifications;

namespace Imaar.Medias
{
    public partial interface IMediasAppService
    {
        //Write your custom code here...
        Task<MediaDto> GetFirstMediaByEntityIdAsync(Guid entityId, MediaEntityType sourceEntityType);
    }
}