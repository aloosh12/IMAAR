using Imaar.Shared;
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

        Task<PagedResultDto<MediaWithNavigationPropertiesDto>> GetListAsync(GetMediasInput input);

        Task<MediaWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<MediaDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetImaarServiceLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetVacancyLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetStoryLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<MediaDto> CreateAsync(MediaCreateDto input);

        Task<MediaDto> UpdateAsync(Guid id, MediaUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(MediaExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> mediaIds);

        Task DeleteAllAsync(GetMediasInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}