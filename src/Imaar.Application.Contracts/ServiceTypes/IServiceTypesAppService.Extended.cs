using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Imaar.MobileResponses;
using System.Collections.Generic;

namespace Imaar.ServiceTypes
{
    public partial interface IServiceTypesAppService
    {
        //Write your custom code here...
        Task<List<ServiceTypeDto>> GetBuildingServiceTypesByCategoryAsync();
        
        Task<List<ServiceTypeDto>> GetVacancyServiceTypesAsync();
        
        Task<List<ServiceTypeDto>> GetImaarServiceTypesAsync();
    }
}