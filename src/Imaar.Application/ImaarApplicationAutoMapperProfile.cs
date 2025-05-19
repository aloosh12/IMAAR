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
using Imaar.Tickets;
using Imaar.Medias;
using Imaar.Stories;
using Imaar.StoryLovers;
using Imaar.Vacancies;
using Imaar.MimeTypes;
using Imaar.UserEvalauations;

namespace Imaar;

public class ImaarApplicationAutoMapperProfile : Profile
{
    public ImaarApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */


        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.Icon, opt => opt.MapFrom(src => $"{MimeTypeMap.GetAttachmentPath()}/CategoryImages/{src.Icon}"));
        CreateMap<Category, CategoryExcelDto>();

        CreateMap<UserProfile, UserProfileDto>()
             .ForMember(dest => dest.ProfilePhoto, opt => opt.MapFrom(src => $"{MimeTypeMap.GetAttachmentPath()}/UserProfileImages/{src.ProfilePhoto}"));
        CreateMap<UserProfile, UserProfileExcelDto>();
        CreateMap<ServiceType, ServiceTypeDto>()
            .ForMember(dest => dest.Icon, opt => opt.MapFrom(src => $"{MimeTypeMap.GetAttachmentPath()}/ServiceTypeImages/{src.Icon}"));


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


        CreateMap<Ticket, TicketDto>();
        CreateMap<Ticket, TicketExcelDto>();
        CreateMap<TicketWithNavigationProperties, TicketWithNavigationPropertiesDto>();
        CreateMap<TicketType, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Title));


        CreateMap<Story, StoryDto>();
        CreateMap<Story, StoryExcelDto>();
        CreateMap<StoryWithNavigationProperties, StoryWithNavigationPropertiesDto>();
        //CreateMap<StoryWithNavigationProperties, StoryMobileDto>()
        //   .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Story.Title))
        //   .ForMember(dest => dest.FromTime, opt => opt.MapFrom(src => src.Story.FromTime))
        //   .ForMember(dest => dest.ExpiryTime, opt => opt.MapFrom(src => src.Story.ExpiryTime))
        //   .ForMember(dest => dest.StoryPublisher, opt => opt.MapFrom(src => src.StoryPublisher.))
        CreateMap<StoryLover, StoryLoverDto>();
        CreateMap<StoryLover, StoryLoverExcelDto>();
        CreateMap<StoryLoverWithNavigationProperties, StoryLoverWithNavigationPropertiesDto>();
        CreateMap<Story, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Title));

        CreateMap<Vacancy, VacancyDto>();
        CreateMap<Vacancy, VacancyExcelDto>();
        CreateMap<VacancyWithNavigationProperties, VacancyWithNavigationPropertiesDto>();

        CreateMap<Media, MediaDto>();
        CreateMap<Media, MediaExcelDto>();
        CreateMap<MediaWithNavigationProperties, MediaWithNavigationPropertiesDto>();
        CreateMap<ImaarService, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Title));
        CreateMap<Vacancy, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Title));


        CreateMap<UserEvalauation, UserEvalauationDto>();
        CreateMap<UserEvalauation, UserEvalauationExcelDto>();
        CreateMap<UserEvalauationWithNavigationProperties, UserEvalauationWithNavigationPropertiesDto>();
    }
}
