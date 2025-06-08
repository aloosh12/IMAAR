using AutoMapper;
using Imaar.BuildingFacades;
using Imaar.Buildings;
using Imaar.Categories;
using Imaar.Cities;
using Imaar.FurnishingLevels;
using Imaar.ImaarServices;
using Imaar.MainAmenities;
using Imaar.Medias;
using Imaar.MimeTypes;
using Imaar.MobileResponses;
using Imaar.Notifications;
using Imaar.NotificationTypes;
using Imaar.Regions;
using Imaar.SecondaryAmenities;
using Imaar.ServiceEvaluations;
using Imaar.ServiceTickets;
using Imaar.ServiceTicketTypes;
using Imaar.ServiceTypes;
using Imaar.Shared;
using Imaar.Stories;
using Imaar.StoryLovers;
using Imaar.StoryTickets;
using Imaar.StoryTicketTypes;
using Imaar.Tickets;
using Imaar.TicketTypes;
using Imaar.UserEvalauations;
using Imaar.UserFollows;
using Imaar.UserProfiles;
using Imaar.UserSavedItems;
using Imaar.UserWorksExhibitions;
using Imaar.Vacancies;
using Imaar.VacancyAdditionalFeatures;
using Imaar.VerificationCodes;
using System;
using System.Collections.Generic;
using System.Linq;

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
        CreateMap<UserProfileWithDetails, UserProfileWithDetailsDto>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserProfile.Id))
             .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.IdentityUser.Name))
             .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.IdentityUser.Surname))
             .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.IdentityUser.PhoneNumber))
             .ForMember(dest => dest.ProfilePhoto, opt => opt.MapFrom(src =>  $"{MimeTypeMap.GetAttachmentPath()}/UserProfileImages/{src.UserProfile.ProfilePhoto}"))
             .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Role))
             .ForMember(dest => dest.SecurityCode, opt => opt.MapFrom(src => src.UserProfile.SecurityNumber))
             .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.IdentityUser.Email))
             .ForMember(dest => dest.BiologicalSex, opt => opt.MapFrom(src => src.UserProfile.BiologicalSex))
             .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.UserProfile.DateOfBirth))
             .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.UserProfile.Latitude))
             .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.UserProfile.Longitude))
             .ForMember(dest => dest.UserWorksExhibitionDtos, opt => opt.MapFrom(src => src.UserWorksExhibitionList))
             .ForMember(dest => dest.SpeedOfCompletion, opt => opt.MapFrom(src => src.SpeedOfCompletion))
             .ForMember(dest => dest.Dealing, opt => opt.MapFrom(src => src.Dealing))
             .ForMember(dest => dest.Cleanliness, opt => opt.MapFrom(src => src.Cleanliness))
             .ForMember(dest => dest.Perfection, opt => opt.MapFrom(src => src.Perfection))
             .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));

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
        CreateMap<StoryWithNavigationProperties, StoryMobileDto>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Story.Id))
           .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Story.Title))
           .ForMember(dest => dest.FromTime, opt => opt.MapFrom(src => src.Story.FromTime))
           .ForMember(dest => dest.ExpiryTime, opt => opt.MapFrom(src => src.Story.ExpiryTime))
           .ForMember(dest => dest.StoryPublisher, opt => opt.MapFrom(src => $"{src.StoryPublisher.FirstName} {src.StoryPublisher.LastName}"));
        CreateMap<StoryLover, StoryLoverDto>();
        CreateMap<StoryLover, StoryLoverExcelDto>();
        CreateMap<StoryLoverWithNavigationProperties, StoryLoverWithNavigationPropertiesDto>();
        CreateMap<Story, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Title));

        CreateMap<Vacancy, VacancyDto>();
        CreateMap<Vacancy, VacancyExcelDto>();
        CreateMap<VacancyWithNavigationProperties, VacancyWithNavigationPropertiesDto>();

        CreateMap<Media, MediaDto>()
            .ForMember(dest => dest.File, opt => opt.MapFrom(src => $"{MimeTypeMap.GetAttachmentPath()}/MediaImages/{src.File}")); ;
        CreateMap<Media, MediaExcelDto>();
        CreateMap<MediaWithNavigationProperties, MediaWithNavigationPropertiesDto>();
        CreateMap<ImaarService, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Title));
        CreateMap<Vacancy, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Title));


        CreateMap<UserEvalauation, UserEvalauationDto>();
        CreateMap<UserEvalauation, UserEvalauationExcelDto>();
        CreateMap<UserEvalauationWithNavigationProperties, UserEvalauationWithNavigationPropertiesDto>();


        CreateMap<ServiceEvaluation, ServiceEvaluationDto>();
        CreateMap<ServiceEvaluation, ServiceEvaluationExcelDto>();
        CreateMap<ServiceEvaluationWithNavigationProperties, ServiceEvaluationWithNavigationPropertiesDto>();

        CreateMap<UserWorksExhibition, UserWorksExhibitionDto>()
            .ForMember(dest => dest.File, opt => opt.MapFrom(src => $"{MimeTypeMap.GetAttachmentPath()}/UserWorksExhibitionImages/{src.File}"));
      //  CreateMap<List<UserWorksExhibition>, List<UserWorksExhibitionDto>>();
        CreateMap<UserWorksExhibition, UserWorksExhibitionExcelDto>();
        CreateMap<UserWorksExhibitionWithNavigationProperties, UserWorksExhibitionWithNavigationPropertiesDto>();


        CreateMap<UserFollow, UserFollowDto>();
        CreateMap<UserFollow, UserFollowExcelDto>();
        CreateMap<UserFollowWithNavigationProperties, UserFollowWithNavigationPropertiesDto>();


        CreateMap<City, CityDto>();
        CreateMap<City, CityExcelDto>();

        CreateMap<Region, RegionDto>();
        CreateMap<Region, RegionExcelDto>();
        CreateMap<RegionWithNavigationProperties, RegionWithNavigationPropertiesDto>();
        CreateMap<City, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

        CreateMap<FurnishingLevel, FurnishingLevelDto>();
        CreateMap<FurnishingLevel, FurnishingLevelExcelDto>();

        CreateMap<BuildingFacade, BuildingFacadeDto>();
        CreateMap<BuildingFacade, BuildingFacadeExcelDto>();

        CreateMap<MainAmenity, MainAmenityDto>();
        CreateMap<MainAmenity, MainAmenityExcelDto>();

        CreateMap<SecondaryAmenity, SecondaryAmenityDto>();
        CreateMap<SecondaryAmenity, SecondaryAmenityExcelDto>();

        CreateMap<Building, BuildingDto>();
        CreateMap<Building, BuildingExcelDto>();
        CreateMap<BuildingWithNavigationProperties, BuildingWithNavigationPropertiesDto>();
        CreateMap<Region, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
        CreateMap<FurnishingLevel, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
        CreateMap<BuildingFacade, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
        CreateMap<MainAmenity, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
        CreateMap<SecondaryAmenity, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));


        CreateMap<ServiceTicketType, ServiceTicketTypeDto>();
        CreateMap<ServiceTicketType, ServiceTicketTypeExcelDto>();

        CreateMap<NotificationType, NotificationTypeDto>();
        CreateMap<NotificationType, NotificationTypeExcelDto>();

        CreateMap<Notification, NotificationDto>();
        CreateMap<Notification, NotificationExcelDto>();
        CreateMap<NotificationWithNavigationProperties, NotificationWithNavigationPropertiesDto>();
        CreateMap<NotificationType, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Title));

        CreateMap<ServiceTicket, ServiceTicketDto>();
        CreateMap<ServiceTicket, ServiceTicketExcelDto>();
        CreateMap<ServiceTicketWithNavigationProperties, ServiceTicketWithNavigationPropertiesDto>();
        CreateMap<ServiceTicketType, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Title));

        CreateMap<StoryTicketType, StoryTicketTypeDto>();
        CreateMap<StoryTicketType, StoryTicketTypeExcelDto>();

        CreateMap<StoryTicket, StoryTicketDto>();
        CreateMap<StoryTicket, StoryTicketExcelDto>();
        CreateMap<StoryTicketWithNavigationProperties, StoryTicketWithNavigationPropertiesDto>();

        CreateMap<UserSavedItem, UserSavedItemDto>();
        CreateMap<UserSavedItem, UserSavedItemExcelDto>();
        CreateMap<UserSavedItemWithNavigationProperties, UserSavedItemWithNavigationPropertiesDto>();


        CreateMap<VacancyAdditionalFeature, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
    }
}
