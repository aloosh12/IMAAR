using System.Threading.Tasks;
using Imaar.MobileResponses;

namespace Imaar.Buildings
{
    public partial interface IBuildingsAppService
    {
        //Write your custom code here...
        Task<MobileResponseDto> CreateWithFilesAsync(BuildingCreateWithFilesDto input);
    }
}