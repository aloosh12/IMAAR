using Imaar.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Advertisements
{
    public partial interface IAdvertisementsAppService : IApplicationService
    {

        Task<PagedResultDto<AdvertisementWithNavigationPropertiesDto>> GetListAsync(GetAdvertisementsInput input);

        Task<AdvertisementWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<AdvertisementDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<AdvertisementDto> CreateAsync(AdvertisementCreateDto input);

        Task<AdvertisementDto> UpdateAsync(Guid id, AdvertisementUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(AdvertisementExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> advertisementIds);

        Task DeleteAllAsync(GetAdvertisementsInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}