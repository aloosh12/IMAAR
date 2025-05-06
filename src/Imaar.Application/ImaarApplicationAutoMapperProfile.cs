using AutoMapper;
using Imaar.Categories;
using Imaar.UserProfiles;

namespace Imaar;

public class ImaarApplicationAutoMapperProfile : Profile
{
    public ImaarApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */


        CreateMap<Category, CategoryDto>();
        CreateMap<Category, CategoryExcelDto>();

        CreateMap<UserProfile, UserProfileDto>();
        CreateMap<UserProfile, UserProfileExcelDto>();
    }
}
