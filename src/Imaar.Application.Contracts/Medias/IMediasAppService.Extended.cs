using System.Collections.Generic;
using System.Threading.Tasks;

namespace Imaar.Medias
{
    public partial interface IMediasAppService
    {
        //Write your custom code here...
        Task<List<MediaDto>> BulkUpdateMediasAsync(MediaBulkUpdateDto input);
    }
}