using System;
using System.Threading.Tasks;
using Imaar.MobileResponses;

namespace Imaar.Vacancies
{
    public partial interface IVacanciesAppService
    {
        //Write your custom code here...
        Task<MobileResponseDto> CreateWithFilesAsync(VacancyCreateWithFilesDto input);
        
        Task<MobileResponseDto> GetVacancyWithDetailsAsync(Guid id);
        
        Task<MobileResponseDto> IncrementViewCounterAsync(Guid id);
        
        Task<MobileResponseDto> IncrementOrderCounterAsync(Guid id);
    }
}