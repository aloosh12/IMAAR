using Volo.Abp.AutoMapper;
using Imaar.Categories;
using AutoMapper;
using Imaar.UserProfiles;
using Imaar.ServiceTypes;
using Imaar.VerificationCodes;
using Imaar.TicketTypes;

namespace Imaar.Blazor;

public class ImaarBlazorAutoMapperProfile : Profile
{
    public ImaarBlazorAutoMapperProfile()
    {
        //Define your AutoMapper configuration here for the Blazor project.

        CreateMap<CategoryDto, CategoryUpdateDto>();
        CreateMap<UserProfileDto, UserProfileUpdateDto>();
        CreateMap<ServiceTypeDto, ServiceTypeUpdateDto>();
        CreateMap<VerificationCodeDto, VerificationCodeUpdateDto>();

        CreateMap<TicketTypeDto, TicketTypeUpdateDto>();
    }
}