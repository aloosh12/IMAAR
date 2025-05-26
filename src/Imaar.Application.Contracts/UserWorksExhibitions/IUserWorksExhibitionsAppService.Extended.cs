using System.Threading.Tasks;

namespace Imaar.UserWorksExhibitions
{
    public partial interface IUserWorksExhibitionsAppService
    {
        //Write your custom code here...
        Task<UserWorksExhibitionDto> CreateMobileAsync(UserWorksExhibitionMobileCreateDto input);

    }
}