using System;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Uow;
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
using Imaar.Categories;
using Imaar.UserProfiles;
using Imaar.ServiceTypes;
using Imaar.ImaarServices;
using Imaar.VerificationCodes;
using Imaar.TicketTypes;
using Imaar.Tickets;
using Imaar.Medias;
using Imaar.Stories;
using Imaar.StoryLovers;
using Imaar.Vacancies;
using Imaar.UserEvalauations;
using Imaar.ServiceEvaluations;

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




        });

        Configure<AbpDbContextOptions>(options =>
        {
                /* The main point to change your DBMS.
                 * See also ImaarMigrationsDbContextFactory for EF Core tooling. */
            options.UseSqlServer();
        });

    }
}
