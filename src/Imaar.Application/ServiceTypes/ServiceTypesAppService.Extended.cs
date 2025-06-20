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
using Imaar.MobileResponses;
using Imaar.Categories;

namespace Imaar.ServiceTypes
{
    public class ServiceTypesAppService : ServiceTypesAppServiceBase, IServiceTypesAppService
    {
        //<suite-custom-code-autogenerated>
        public ServiceTypesAppService(IServiceTypeRepository serviceTypeRepository, ServiceTypeManager serviceTypeManager)
            : base(serviceTypeRepository, serviceTypeManager)
        {
        }
        //</suite-custom-code-autogenerated>

        //Write your custom code...

        [AllowAnonymous]
        public async Task<List<ServiceTypeDto>> GetBuildingServiceTypesByCategoryAsync()
        {
            Guid categoryId = Guid.Parse("403b9deb-0805-b321-cd0a-3a19c0e95438");
            var serviceTypes = await _serviceTypeRepository.GetListByCategoryIdAsync(
                categoryId,
                null,
                1000,
                0);
            return ObjectMapper.Map<List<ServiceType>, List<ServiceTypeDto>>(serviceTypes);
           
        }
        
        [AllowAnonymous]
        public async Task<List<ServiceTypeDto>> GetVacancyServiceTypesAsync()
        {
            // Use GetListByCategoryIdAsync for filtering by category, or customize your query if needed
            // For this example, I'm assuming there's a specific category for vacancy service types
            // You might need to adjust the logic based on your business needs
            Guid categoryId = Guid.Parse("1b5c7398-ab5f-5a65-4d41-3a19e3aafec8");
            var serviceTypes = await _serviceTypeRepository.GetListByCategoryIdAsync(
                categoryId,
                null,
                1000,
                0);
            return ObjectMapper.Map<List<ServiceType>, List<ServiceTypeDto>>(serviceTypes);

        }

        [AllowAnonymous]
        public async Task<List<ServiceTypeDto>> GetImaarServiceTypesAsync()
        {
            // Similar to the vacancy method but with different filtering
            Guid categoryId = Guid.Parse("c2e51c10-d389-d9ba-45bb-3a19ad0ed525");
            var serviceTypes = await _serviceTypeRepository.GetListByCategoryIdAsync(
                categoryId,
                null,
                1000,
                0);
            return ObjectMapper.Map<List<ServiceType>, List<ServiceTypeDto>>(serviceTypes);
        }
    }
}