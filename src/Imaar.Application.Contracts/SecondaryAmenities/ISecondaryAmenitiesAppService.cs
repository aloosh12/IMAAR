using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.SecondaryAmenities
{
    public partial interface ISecondaryAmenitiesAppService : IApplicationService
    {

        Task<PagedResultDto<SecondaryAmenityDto>> GetListAsync(GetSecondaryAmenitiesInput input);

        Task<SecondaryAmenityDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<SecondaryAmenityDto> CreateAsync(SecondaryAmenityCreateDto input);

        Task<SecondaryAmenityDto> UpdateAsync(Guid id, SecondaryAmenityUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(SecondaryAmenityExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> secondaryamenityIds);

        Task DeleteAllAsync(GetSecondaryAmenitiesInput input);
        Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}