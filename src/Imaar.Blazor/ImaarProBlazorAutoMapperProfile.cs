using Volo.Abp.AutoMapper;
using Imaar.Categories;
using AutoMapper;
using Imaar.UserProfiles;

namespace Imaar.Blazor;

public class ImaarBlazorAutoMapperProfile : Profile
{
    public ImaarBlazorAutoMapperProfile()
    {
        //Define your AutoMapper configuration here for the Blazor project.

        CreateMap<CategoryDto, CategoryUpdateDto>();
        CreateMap<UserProfileDto, UserProfileUpdateDto>();
    }
}