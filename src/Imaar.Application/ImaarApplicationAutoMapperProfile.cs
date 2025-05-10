using AutoMapper;
using Imaar.Categories;
using Imaar.UserProfiles;
using Imaar.ServiceTypes;
using Imaar.Shared;
using Imaar.ImaarServices;
using System;
using Imaar.MobileResponses;
using Imaar.VerificationCodes;
using Imaar.TicketTypes;

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
        CreateMap<ServiceType, ServiceTypeDto>();


        CreateMap<ImaarService, ImaarServiceDto>();
        CreateMap<ImaarService, ImaarServiceExcelDto>();
        CreateMap<ImaarServiceWithNavigationProperties, ImaarServiceWithNavigationPropertiesDto>();
        CreateMap<ServiceType, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Title));
        CreateMap<UserProfile, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.SecurityNumber));

        CreateMap<MobileResponse, MobileResponseDto>();

        CreateMap<VerificationCode, VerificationCodeDto>();
        CreateMap<VerificationCode, VerificationCodeExcelDto>();

        CreateMap<TicketType, TicketTypeDto>();
        CreateMap<TicketType, TicketTypeExcelDto>();
    }
}
