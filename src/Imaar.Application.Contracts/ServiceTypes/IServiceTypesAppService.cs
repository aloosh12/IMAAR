using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Imaar.ServiceTypes
{
    public partial interface IServiceTypesAppService : IApplicationService
    {

        Task<PagedResultDto<ServiceTypeDto>> GetListByCategoryIdAsync(GetServiceTypeListInput input);

        Task<PagedResultDto<ServiceTypeDto>> GetListAsync(GetServiceTypesInput input);

        Task<ServiceTypeDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<ServiceTypeDto> CreateAsync(ServiceTypeCreateDto input);

        Task<ServiceTypeDto> UpdateAsync(Guid id, ServiceTypeUpdateDto input);
    }
}