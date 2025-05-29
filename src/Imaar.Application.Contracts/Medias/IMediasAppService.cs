using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Medias
{
    public partial interface IMediasAppService : IApplicationService
    {

        Task<PagedResultDto<MediaDto>> GetListAsync(GetMediasInput input);

        Task<MediaDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<MediaDto> CreateAsync(MediaCreateDto input);

        Task<MediaDto> UpdateAsync(Guid id, MediaUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(MediaExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> mediaIds);

        Task DeleteAllAsync(GetMediasInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}