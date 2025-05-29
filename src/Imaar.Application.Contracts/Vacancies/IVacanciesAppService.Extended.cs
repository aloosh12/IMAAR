using System.Threading.Tasks;
using Imaar.MobileResponses;

namespace Imaar.Vacancies
{
    public partial interface IVacanciesAppService
    {
        //Write your custom code here...
        Task<MobileResponseDto> CreateWithFilesAsync(VacancyCreateWithFilesDto input);
    }
}