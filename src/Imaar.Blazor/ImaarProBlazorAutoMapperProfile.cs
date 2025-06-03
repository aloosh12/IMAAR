using AutoMapper;
using Imaar.BuildingFacades;
using Imaar.Buildings;
using Imaar.Categories;
using Imaar.Cities;
using Imaar.FurnishingLevels;
using Imaar.MainAmenities;
using Imaar.Medias;
using Imaar.Notifications;
using Imaar.NotificationTypes;
using Imaar.Regions;
using Imaar.SecondaryAmenities;
using Imaar.ServiceEvaluations;
using Imaar.ServiceTickets;
using Imaar.ServiceTicketTypes;
using Imaar.ServiceTypes;
using Imaar.Stories;
using Imaar.StoryLovers;
using Imaar.Tickets;
using Imaar.TicketTypes;
using Imaar.UserEvalauations;
using Imaar.UserFollows;
using Imaar.UserProfiles;
using Imaar.UserWorksExhibitions;
using Imaar.Vacancies;
using Imaar.VerificationCodes;
using Imaar.StoryTickets;
using Imaar.StoryTicketTypes;
using Volo.Abp.AutoMapper;

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
        CreateMap<TicketDto, TicketUpdateDto>();

        CreateMap<StoryDto, StoryUpdateDto>();

        CreateMap<StoryLoverDto, StoryLoverUpdateDto>();

        CreateMap<VacancyDto, VacancyUpdateDto>();

        CreateMap<MediaDto, MediaUpdateDto>();
        CreateMap<UserEvalauationDto, UserEvalauationUpdateDto>();
        CreateMap<ServiceEvaluationDto, ServiceEvaluationUpdateDto>();

        CreateMap<UserWorksExhibitionDto, UserWorksExhibitionUpdateDto>();
        CreateMap<UserFollowDto, UserFollowUpdateDto>();

        CreateMap<CityDto, CityUpdateDto>();

        CreateMap<RegionDto, RegionUpdateDto>();

        CreateMap<FurnishingLevelDto, FurnishingLevelUpdateDto>();

        CreateMap<BuildingFacadeDto, BuildingFacadeUpdateDto>();

        CreateMap<MainAmenityDto, MainAmenityUpdateDto>();

        CreateMap<SecondaryAmenityDto, SecondaryAmenityUpdateDto>();

        CreateMap<BuildingDto, BuildingUpdateDto>().Ignore(x => x.MainAmenityIds).Ignore(x => x.SecondaryAmenityIds);

        CreateMap<ServiceTicketTypeDto, ServiceTicketTypeUpdateDto>();

        CreateMap<NotificationTypeDto, NotificationTypeUpdateDto>();

        CreateMap<NotificationDto, NotificationUpdateDto>();

        CreateMap<ServiceTicketDto, ServiceTicketUpdateDto>();

        CreateMap<StoryTicketTypeDto, StoryTicketTypeUpdateDto>();

        CreateMap<StoryTicketDto, StoryTicketUpdateDto>();
    }
}