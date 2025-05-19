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
    }
}