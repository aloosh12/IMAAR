using AutoMapper;
using Imaar.Advertisements;
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
using Imaar.Vacancies;
using Imaar.VacancyAdditionalFeatures;
using Imaar.VerificationCodes;
using Imaar.BuildingEvaluations;
using Imaar.VacancyEvaluations;
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

        CreateMap<UserSavedItemDto, UserSavedItemUpdateDto>();

        CreateMap<AdvertisementDto, AdvertisementUpdateDto>();
        CreateMap<VacancyAdditionalFeatureDto, VacancyAdditionalFeatureUpdateDto>();

        CreateMap<VacancyDto, VacancyUpdateDto>().Ignore(x => x.VacancyAdditionalFeatureIds);

        CreateMap<BuildingEvaluationDto, BuildingEvaluationUpdateDto>();

        CreateMap<VacancyEvaluationDto, VacancyEvaluationUpdateDto>();
    }
}