using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Imaar.Permissions;
using Imaar.ServiceTypes;

namespace Imaar.ServiceTypes
{

    [Authorize(ImaarPermissions.ServiceTypes.Default)]
    public abstract class ServiceTypesAppServiceBase : ImaarAppService
    {

        protected IServiceTypeRepository _serviceTypeRepository;
        protected ServiceTypeManager _serviceTypeManager;

        public ServiceTypesAppServiceBase(IServiceTypeRepository serviceTypeRepository, ServiceTypeManager serviceTypeManager)
        {

            _serviceTypeRepository = serviceTypeRepository;
            _serviceTypeManager = serviceTypeManager;

        }

        public virtual async Task<PagedResultDto<ServiceTypeDto>> GetListByCategoryIdAsync(GetServiceTypeListInput input)
        {
            var serviceTypes = await _serviceTypeRepository.GetListByCategoryIdAsync(
                input.CategoryId,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount);

            return new PagedResultDto<ServiceTypeDto>
            {
                TotalCount = await _serviceTypeRepository.GetCountByCategoryIdAsync(input.CategoryId),
                Items = ObjectMapper.Map<List<ServiceType>, List<ServiceTypeDto>>(serviceTypes)
            };
        }

        public virtual async Task<PagedResultDto<ServiceTypeDto>> GetListAsync(GetServiceTypesInput input)
        {
            var totalCount = await _serviceTypeRepository.GetCountAsync(input.FilterText, input.Title, input.Icon, input.OrderMin, input.OrderMax, input.IsActive);
            var items = await _serviceTypeRepository.GetListAsync(input.FilterText, input.Title, input.Icon, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<ServiceTypeDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<ServiceType>, List<ServiceTypeDto>>(items)
            };
        }

        public virtual async Task<ServiceTypeDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<ServiceType, ServiceTypeDto>(await _serviceTypeRepository.GetAsync(id));
        }

        [Authorize(ImaarPermissions.ServiceTypes.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _serviceTypeRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.ServiceTypes.Create)]
        public virtual async Task<ServiceTypeDto> CreateAsync(ServiceTypeCreateDto input)
        {

            var serviceType = await _serviceTypeManager.CreateAsync(input.CategoryId
            , input.Title, input.Order, input.IsActive, input.Icon
            );

            return ObjectMapper.Map<ServiceType, ServiceTypeDto>(serviceType);
        }

        [Authorize(ImaarPermissions.ServiceTypes.Edit)]
        public virtual async Task<ServiceTypeDto> UpdateAsync(Guid id, ServiceTypeUpdateDto input)
        {

            var serviceType = await _serviceTypeManager.UpdateAsync(
            id, input.CategoryId
            , input.Title, input.Order, input.IsActive, input.Icon
            );

            return ObjectMapper.Map<ServiceType, ServiceTypeDto>(serviceType);
        }
    }
}