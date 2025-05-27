using Volo.Abp.AutoMapper;
using Imaar.Categories;
using AutoMapper;
using Imaar.UserProfiles;
using Imaar.ServiceTypes;
using Imaar.VerificationCodes;
using Imaar.TicketTypes;
using Imaar.Tickets;
using Imaar.Medias;
using Imaar.Stories;
using Imaar.StoryLovers;
using Imaar.Vacancies;
using Imaar.UserEvalauations;
using Imaar.ServiceEvaluations;
using Imaar.UserWorksExhibitions;
using Imaar.UserFollows;
using Imaar.BuildingFacades;
using Imaar.Buildings;
using Imaar.Cities;
using Imaar.FurnishingLevels;
using Imaar.MainAmenities;
using Imaar.Regions;
using Imaar.SecondaryAmenities;

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
    }
}