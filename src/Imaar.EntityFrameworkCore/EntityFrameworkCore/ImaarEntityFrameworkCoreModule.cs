using Imaar.Advertisements;
using Imaar.BuildingFacades;
using Imaar.Buildings;
using Imaar.Categories;
using Imaar.Cities;
using Imaar.FurnishingLevels;
using Imaar.ImaarServices;
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
using Imaar.VerificationCodes;
using Imaar.VacancyAdditionalFeatures;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.Uow;

namespace Imaar.EntityFrameworkCore;

[DependsOn(
    typeof(ImaarDomainModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpOpenIddictEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqlServerModule),
    typeof(AbpBackgroundJobsEntityFrameworkCoreModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule),
    typeof(AbpTenantManagementEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule)
    )]
public class ImaarEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        ImaarEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<ImaarDbContext>(options =>
        {
                /* Remove "includeAllEntities: true" to create
                 * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
            options.AddRepository<Category, Categories.EfCoreCategoryRepository>(); 
            options.AddRepository<UserProfile, UserProfiles.EfCoreUserProfileRepository>();
            options.AddRepository<ServiceType, ServiceTypes.EfCoreServiceTypeRepository>();
            options.AddRepository<ImaarService, ImaarServices.EfCoreImaarServiceRepository>();
            options.AddRepository<VerificationCode, VerificationCodes.EfCoreVerificationCodeRepository>();
            options.AddRepository<TicketType, TicketTypes.EfCoreTicketTypeRepository>(); 
            options.AddRepository<Ticket, Tickets.EfCoreTicketRepository>();
            options.AddRepository<Story, Stories.EfCoreStoryRepository>();
            options.AddRepository<StoryLover, StoryLovers.EfCoreStoryLoverRepository>();
            options.AddRepository<Vacancy, Vacancies.EfCoreVacancyRepository>();
            options.AddRepository<Media, Medias.EfCoreMediaRepository>(); 
            options.AddRepository<UserEvalauation, UserEvalauations.EfCoreUserEvalauationRepository>();
            options.AddRepository<ServiceEvaluation, ServiceEvaluations.EfCoreServiceEvaluationRepository>();
            options.AddRepository<UserWorksExhibition, UserWorksExhibitions.EfCoreUserWorksExhibitionRepository>();
            options.AddRepository<UserFollow, UserFollows.EfCoreUserFollowRepository>();

            options.AddRepository<City, Cities.EfCoreCityRepository>();
            options.AddRepository<Region, Regions.EfCoreRegionRepository>();

            options.AddRepository<FurnishingLevel, FurnishingLevels.EfCoreFurnishingLevelRepository>();

            options.AddRepository<BuildingFacade, BuildingFacades.EfCoreBuildingFacadeRepository>();

            options.AddRepository<MainAmenity, MainAmenities.EfCoreMainAmenityRepository>();

            options.AddRepository<SecondaryAmenity, SecondaryAmenities.EfCoreSecondaryAmenityRepository>();

            options.AddRepository<Building, Buildings.EfCoreBuildingRepository>();
            options.AddRepository<ServiceTicketType, ServiceTicketTypes.EfCoreServiceTicketTypeRepository>();

            options.AddRepository<NotificationType, NotificationTypes.EfCoreNotificationTypeRepository>();

            options.AddRepository<Notification, Notifications.EfCoreNotificationRepository>();

            options.AddRepository<ServiceTicket, ServiceTickets.EfCoreServiceTicketRepository>();
            options.AddRepository<StoryTicketType, StoryTicketTypes.EfCoreStoryTicketTypeRepository>();

            options.AddRepository<StoryTicket, StoryTickets.EfCoreStoryTicketRepository>();
            options.AddRepository<UserSavedItem, UserSavedItems.EfCoreUserSavedItemRepository>();
            options.AddRepository<Advertisement, Advertisements.EfCoreAdvertisementRepository>();
            options.AddRepository<VacancyAdditionalFeature, VacancyAdditionalFeatures.EfCoreVacancyAdditionalFeatureRepository>();



        });

        Configure<AbpDbContextOptions>(options =>
        {
                /* The main point to change your DBMS.
                 * See also ImaarMigrationsDbContextFactory for EF Core tooling. */
            options.UseSqlServer();
        });

    }
}
