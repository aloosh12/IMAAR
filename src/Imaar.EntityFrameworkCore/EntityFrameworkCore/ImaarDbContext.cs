using Imaar.Advertisements;
using Imaar.BuildingFacades;
using Imaar.Buildings;
using Imaar.Categories;
using Imaar.Cities;
using Imaar.FurnishingLevels;
using Imaar.ImaarServices;
using Imaar.MainAmenities;
using Imaar.Medias;
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
using Imaar.UserProfiles;
using Imaar.UserSavedItems;
using Imaar.UserWorksExhibitions;
using Imaar.Vacancies;
using Imaar.Vacancies;
using Imaar.VacancyAdditionalFeatures;
using Imaar.VerificationCodes;
using Imaar.VerificationCodes;
using Imaar.Buildings;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Imaar.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class ImaarDbContext :
    AbpDbContext<ImaarDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    public DbSet<VacancyAdditionalFeature> VacancyAdditionalFeatures { get; set; } = null!;

    public DbSet<Advertisement> Advertisements { get; set; } = null!;
    public DbSet<UserSavedItem> UserSavedItems { get; set; } = null!;

    public DbSet<StoryTicket> StoryTickets { get; set; } = null!;
    public DbSet<StoryTicketType> StoryTicketTypes { get; set; } = null!;
    public DbSet<ServiceTicket> ServiceTickets { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<NotificationType> NotificationTypes { get; set; } = null!;
    public DbSet<ServiceTicketType> ServiceTicketTypes { get; set; } = null!;
    public DbSet<Building> Buildings { get; set; } = null!;
    public DbSet<SecondaryAmenity> SecondaryAmenities { get; set; } = null!;
    public DbSet<MainAmenity> MainAmenities { get; set; } = null!;
    public DbSet<BuildingFacade> BuildingFacades { get; set; } = null!;
    public DbSet<FurnishingLevel> FurnishingLevels { get; set; } = null!;
    public DbSet<Region> Regions { get; set; } = null!;
    public DbSet<City> Cities { get; set; } = null!;
    public DbSet<UserFollow> UserFollows { get; set; } = null!;
    public DbSet<UserWorksExhibition> UserWorksExhibitions { get; set; } = null!;
    public DbSet<ServiceEvaluation> ServiceEvaluations { get; set; } = null!;
    public DbSet<UserEvalauation> UserEvalauations { get; set; } = null!;
    public DbSet<Media> Medias { get; set; } = null!;
    public DbSet<Vacancy> Vacancies { get; set; } = null!;
    public DbSet<StoryLover> StoryLovers { get; set; } = null!;
    public DbSet<Story> Stories { get; set; } = null!;
    public DbSet<Ticket> Tickets { get; set; } = null!;
    public DbSet<TicketType> TicketTypes { get; set; } = null!;
    public DbSet<VerificationCode> VerificationCodes { get; set; } = null!;
    public DbSet<ImaarService> ImaarServices { get; set; } = null!;
    public DbSet<ServiceType> ServiceTypes { get; set; } = null!;
    public DbSet<UserProfile> UserProfiles { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }
    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public ImaarDbContext(DbContextOptions<ImaarDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(ImaarConsts.DbTablePrefix + "YourEntities", ImaarConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});

        if (builder.IsHostDatabase())
        {
        }
  
        if (builder.IsHostDatabase())
        {
            builder.Entity<Category>(b =>
            {
                b.ToTable(ImaarConsts.DbTablePrefix + "Categories", ImaarConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Title).HasColumnName(nameof(Category.Title)).IsRequired();
                b.Property(x => x.Icon).HasColumnName(nameof(Category.Icon)).IsRequired();
                b.Property(x => x.Order).HasColumnName(nameof(Category.Order));
                b.Property(x => x.IsActive).HasColumnName(nameof(Category.IsActive));
                b.HasMany(x => x.ServiceTypes).WithOne().HasForeignKey(x => x.CategoryId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<ServiceType>(b =>
            {
                b.ToTable(ImaarConsts.DbTablePrefix + "ServiceTypes", ImaarConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Title).HasColumnName(nameof(ServiceType.Title)).IsRequired();
                b.Property(x => x.Icon).HasColumnName(nameof(ServiceType.Icon));
                b.Property(x => x.Order).HasColumnName(nameof(ServiceType.Order));
                b.Property(x => x.IsActive).HasColumnName(nameof(ServiceType.IsActive));
                b.HasOne<Category>().WithMany(x => x.ServiceTypes).HasForeignKey(x => x.CategoryId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            });

        }

        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<ImaarService>(b =>
            {
                b.ToTable(ImaarConsts.DbTablePrefix + "ImaarServices", ImaarConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Title).HasColumnName(nameof(ImaarService.Title)).IsRequired();
                b.Property(x => x.Description).HasColumnName(nameof(ImaarService.Description)).IsRequired();
                b.Property(x => x.ServiceLocation).HasColumnName(nameof(ImaarService.ServiceLocation)).IsRequired();
                b.Property(x => x.ServiceNumber).HasColumnName(nameof(ImaarService.ServiceNumber)).IsRequired();
                b.Property(x => x.DateOfPublish).HasColumnName(nameof(ImaarService.DateOfPublish));
                b.Property(x => x.Price).HasColumnName(nameof(ImaarService.Price));
                b.Property(x => x.Latitude).HasColumnName(nameof(ImaarService.Latitude));
                b.Property(x => x.Longitude).HasColumnName(nameof(ImaarService.Longitude));
                b.HasOne<ServiceType>().WithMany().IsRequired().HasForeignKey(x => x.ServiceTypeId).OnDelete(DeleteBehavior.NoAction);
                b.HasOne<UserProfile>().WithMany().IsRequired().HasForeignKey(x => x.UserProfileId).OnDelete(DeleteBehavior.NoAction);
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<VerificationCode>(b =>
            {
                b.ToTable(ImaarConsts.DbTablePrefix + "VerificationCodes", ImaarConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.PhoneNumber).HasColumnName(nameof(VerificationCode.PhoneNumber)).IsRequired();
                b.Property(x => x.SecurityCode).HasColumnName(nameof(VerificationCode.SecurityCode));
                b.Property(x => x.IsFinish).HasColumnName(nameof(VerificationCode.IsFinish));
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<TicketType>(b =>
            {
                b.ToTable(ImaarConsts.DbTablePrefix + "TicketTypes", ImaarConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Title).HasColumnName(nameof(TicketType.Title)).IsRequired();
                b.Property(x => x.Order).HasColumnName(nameof(TicketType.Order));
                b.Property(x => x.IsActive).HasColumnName(nameof(TicketType.IsActive));
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<Ticket>(b =>
            {
                b.ToTable(ImaarConsts.DbTablePrefix + "Tickets", ImaarConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Description).HasColumnName(nameof(Ticket.Description)).IsRequired();
                b.HasOne<TicketType>().WithMany().IsRequired().HasForeignKey(x => x.TicketTypeId).OnDelete(DeleteBehavior.NoAction);
                b.HasOne<UserProfile>().WithMany().IsRequired().HasForeignKey(x => x.TicketCreatorId).OnDelete(DeleteBehavior.NoAction);
                b.HasOne<UserProfile>().WithMany().IsRequired().HasForeignKey(x => x.TicketAgainstId).OnDelete(DeleteBehavior.NoAction);
            });

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<Story>(b =>
            {
                b.ToTable(ImaarConsts.DbTablePrefix + "Stories", ImaarConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Title).HasColumnName(nameof(Story.Title));
                b.Property(x => x.FromTime).HasColumnName(nameof(Story.FromTime));
                b.Property(x => x.ExpiryTime).HasColumnName(nameof(Story.ExpiryTime));
                b.HasOne<UserProfile>().WithMany().HasForeignKey(x => x.StoryPublisherId).OnDelete(DeleteBehavior.SetNull);
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<StoryLover>(b =>
            {
                b.ToTable(ImaarConsts.DbTablePrefix + "StoryLovers", ImaarConsts.DbSchema);
                b.ConfigureByConvention();
                b.HasOne<UserProfile>().WithMany().IsRequired().HasForeignKey(x => x.UserProfileId).OnDelete(DeleteBehavior.NoAction);
                b.HasOne<Story>().WithMany().IsRequired().HasForeignKey(x => x.StoryId).OnDelete(DeleteBehavior.NoAction);
            });

        }
        if (builder.IsHostDatabase())
        {
        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<UserEvalauation>(b =>
            {
                b.ToTable(ImaarConsts.DbTablePrefix + "UserEvalauations", ImaarConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.SpeedOfCompletion).HasColumnName(nameof(UserEvalauation.SpeedOfCompletion));
                b.Property(x => x.Dealing).HasColumnName(nameof(UserEvalauation.Dealing));
                b.Property(x => x.Cleanliness).HasColumnName(nameof(UserEvalauation.Cleanliness));
                b.Property(x => x.Perfection).HasColumnName(nameof(UserEvalauation.Perfection));
                b.Property(x => x.Price).HasColumnName(nameof(UserEvalauation.Price));
                b.HasOne<UserProfile>().WithMany().IsRequired().HasForeignKey(x => x.Evaluatord).OnDelete(DeleteBehavior.NoAction);
                b.HasOne<UserProfile>().WithMany().IsRequired().HasForeignKey(x => x.EvaluatedPersonId).OnDelete(DeleteBehavior.NoAction);
            });

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<ServiceEvaluation>(b =>
            {
                b.ToTable(ImaarConsts.DbTablePrefix + "ServiceEvaluations", ImaarConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Rate).HasColumnName(nameof(ServiceEvaluation.Rate));
                b.HasOne<UserProfile>().WithMany().IsRequired().HasForeignKey(x => x.EvaluatorId).OnDelete(DeleteBehavior.NoAction);
                b.HasOne<ImaarService>().WithMany().IsRequired().HasForeignKey(x => x.ImaarServiceId).OnDelete(DeleteBehavior.NoAction);
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<UserWorksExhibition>(b =>
            {
                b.ToTable(ImaarConsts.DbTablePrefix + "UserWorksExhibitions", ImaarConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Title).HasColumnName(nameof(UserWorksExhibition.Title));
                b.Property(x => x.File).HasColumnName(nameof(UserWorksExhibition.File)).IsRequired();
                b.Property(x => x.Order).HasColumnName(nameof(UserWorksExhibition.Order));
                b.HasOne<UserProfile>().WithMany().IsRequired().HasForeignKey(x => x.UserProfileId).OnDelete(DeleteBehavior.NoAction);
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<UserFollow>(b =>
            {
                b.ToTable(ImaarConsts.DbTablePrefix + "UserFollows", ImaarConsts.DbSchema);
                b.ConfigureByConvention();
                b.HasOne<UserProfile>().WithMany().IsRequired().HasForeignKey(x => x.FollowerUserId).OnDelete(DeleteBehavior.NoAction);
                b.HasOne<UserProfile>().WithMany().IsRequired().HasForeignKey(x => x.FollowingUserId).OnDelete(DeleteBehavior.NoAction);
            });

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<MainAmenity>(b =>
            {
                b.ToTable(ImaarConsts.DbTablePrefix + "MainAmenities", ImaarConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Name).HasColumnName(nameof(MainAmenity.Name)).IsRequired();
    b.Property(x => x.Order).HasColumnName(nameof(MainAmenity.Order));
    b.Property(x => x.IsActive).HasColumnName(nameof(MainAmenity.IsActive));
});

        }
        if (builder.IsHostDatabase())
{
    builder.Entity<SecondaryAmenity>(b =>
    {
        b.ToTable(ImaarConsts.DbTablePrefix + "SecondaryAmenities", ImaarConsts.DbSchema);
        b.ConfigureByConvention();
        b.Property(x => x.Name).HasColumnName(nameof(SecondaryAmenity.Name)).IsRequired();
        b.Property(x => x.Order).HasColumnName(nameof(SecondaryAmenity.Order));
        b.Property(x => x.IsActive).HasColumnName(nameof(SecondaryAmenity.IsActive));
    });

}
if (builder.IsHostDatabase())
{
    builder.Entity<City>(b =>
    {
        b.ToTable(ImaarConsts.DbTablePrefix + "Cities", ImaarConsts.DbSchema);
        b.ConfigureByConvention();
        b.Property(x => x.Name).HasColumnName(nameof(City.Name)).IsRequired();
        b.Property(x => x.Order).HasColumnName(nameof(City.Order));
        b.Property(x => x.IsActive).HasColumnName(nameof(City.IsActive));
    });

}
if (builder.IsHostDatabase())
{
    builder.Entity<Region>(b =>
    {
        b.ToTable(ImaarConsts.DbTablePrefix + "Regions", ImaarConsts.DbSchema);
        b.ConfigureByConvention();
        b.Property(x => x.Name).HasColumnName(nameof(Region.Name)).IsRequired();
        b.Property(x => x.Order).HasColumnName(nameof(Region.Order));
        b.Property(x => x.IsActive).HasColumnName(nameof(Region.IsActive));
        b.HasOne<City>().WithMany().IsRequired().HasForeignKey(x => x.CityId).OnDelete(DeleteBehavior.NoAction);
    });

}
if (builder.IsHostDatabase())
{
    builder.Entity<FurnishingLevel>(b =>
    {
        b.ToTable(ImaarConsts.DbTablePrefix + "FurnishingLevels", ImaarConsts.DbSchema);
        b.ConfigureByConvention();
        b.Property(x => x.Name).HasColumnName(nameof(FurnishingLevel.Name)).IsRequired();
        b.Property(x => x.Order).HasColumnName(nameof(FurnishingLevel.Order));
        b.Property(x => x.IsActive).HasColumnName(nameof(FurnishingLevel.IsActive));
    });

}
if (builder.IsHostDatabase())
{
    builder.Entity<BuildingFacade>(b =>
    {
        b.ToTable(ImaarConsts.DbTablePrefix + "BuildingFacades", ImaarConsts.DbSchema);
        b.ConfigureByConvention();
        b.Property(x => x.Name).HasColumnName(nameof(BuildingFacade.Name)).IsRequired();
        b.Property(x => x.Order).HasColumnName(nameof(BuildingFacade.Order));
        b.Property(x => x.IsActive).HasColumnName(nameof(BuildingFacade.IsActive));
    });

}
if (builder.IsHostDatabase())
{

}
if (builder.IsHostDatabase())
{

}
if (builder.IsHostDatabase())
{

}
if (builder.IsHostDatabase())
{

}
if (builder.IsHostDatabase())
{

            if (builder.IsHostDatabase())
            {
                builder.Entity<ServiceTicketType>(b =>
                {
                    b.ToTable(ImaarConsts.DbTablePrefix + "ServiceTicketTypes", ImaarConsts.DbSchema);
                    b.ConfigureByConvention();
                    b.Property(x => x.Title).HasColumnName(nameof(ServiceTicketType.Title)).IsRequired();
                    b.Property(x => x.Order).HasColumnName(nameof(ServiceTicketType.Order));
                    b.Property(x => x.IsActive).HasColumnName(nameof(ServiceTicketType.IsActive));
                });

            }
            if (builder.IsHostDatabase())
            {
                builder.Entity<NotificationType>(b =>
                {
                    b.ToTable(ImaarConsts.DbTablePrefix + "NotificationTypes", ImaarConsts.DbSchema);
                    b.ConfigureByConvention();
                    b.Property(x => x.Title).HasColumnName(nameof(NotificationType.Title)).IsRequired();
                });

            }
            if (builder.IsHostDatabase())
            {
                builder.Entity<Notification>(b =>
                {
                    b.ToTable(ImaarConsts.DbTablePrefix + "Notifications", ImaarConsts.DbSchema);
                    b.ConfigureByConvention();
                    b.Property(x => x.Title).HasColumnName(nameof(Notification.Title)).IsRequired();
                    b.Property(x => x.Message).HasColumnName(nameof(Notification.Message)).IsRequired();
                    b.Property(x => x.IsRead).HasColumnName(nameof(Notification.IsRead));
                    b.Property(x => x.ReadDate).HasColumnName(nameof(Notification.ReadDate));
                    b.Property(x => x.Priority).HasColumnName(nameof(Notification.Priority));
                    b.Property(x => x.SourceEntityType).HasColumnName(nameof(Notification.SourceEntityType));
                    b.Property(x => x.SourceEntityId).HasColumnName(nameof(Notification.SourceEntityId));
                    b.Property(x => x.SenderUserId).HasColumnName(nameof(Notification.SenderUserId));
                    b.HasOne<UserProfile>().WithMany().IsRequired().HasForeignKey(x => x.UserProfileId).OnDelete(DeleteBehavior.NoAction);
                    b.HasOne<NotificationType>().WithMany().IsRequired().HasForeignKey(x => x.NotificationTypeId).OnDelete(DeleteBehavior.NoAction);
                });

            }
            if (builder.IsHostDatabase())
            {

            }
            if (builder.IsHostDatabase())
            {

            }
            if (builder.IsHostDatabase())
            {

            }
            if (builder.IsHostDatabase())
            {
                builder.Entity<ServiceTicket>(b =>
                {
                    b.ToTable(ImaarConsts.DbTablePrefix + "ServiceTickets", ImaarConsts.DbSchema);
                    b.ConfigureByConvention();
                    b.Property(x => x.Description).HasColumnName(nameof(ServiceTicket.Description)).IsRequired();
                    b.Property(x => x.TicketEntityType).HasColumnName(nameof(ServiceTicket.TicketEntityType));
                    b.Property(x => x.TicketEntityId).HasColumnName(nameof(ServiceTicket.TicketEntityId)).IsRequired();
                    b.HasOne<ServiceTicketType>().WithMany().IsRequired().HasForeignKey(x => x.ServiceTicketTypeId).OnDelete(DeleteBehavior.NoAction);
                    b.HasOne<UserProfile>().WithMany().IsRequired().HasForeignKey(x => x.TicketCreatorId).OnDelete(DeleteBehavior.NoAction);
                });
            }

            if (builder.IsHostDatabase())
            {
                builder.Entity<Media>(b =>
                {
                    b.ToTable(ImaarConsts.DbTablePrefix + "Medias", ImaarConsts.DbSchema);
                    b.ConfigureByConvention();
                    b.Property(x => x.Title).HasColumnName(nameof(Media.Title));
                    b.Property(x => x.File).HasColumnName(nameof(Media.File)).IsRequired();
                    b.Property(x => x.Order).HasColumnName(nameof(Media.Order));
                    b.Property(x => x.IsActive).HasColumnName(nameof(Media.IsActive));
                    b.Property(x => x.SourceEntityType).HasColumnName(nameof(Media.SourceEntityType));
                    b.Property(x => x.SourceEntityId).HasColumnName(nameof(Media.SourceEntityId)).IsRequired();
                });

            }

            if (builder.IsHostDatabase())
            {
                builder.Entity<StoryTicketType>(b =>
                {
                    b.ToTable(ImaarConsts.DbTablePrefix + "StoryTicketTypes", ImaarConsts.DbSchema);
                    b.ConfigureByConvention();
                    b.Property(x => x.Title).HasColumnName(nameof(StoryTicketType.Title)).IsRequired();
                    b.Property(x => x.Order).HasColumnName(nameof(StoryTicketType.Order));
                    b.Property(x => x.IsActive).HasColumnName(nameof(StoryTicketType.IsActive));
                });

            }
            if (builder.IsHostDatabase())
            {

            }
            if (builder.IsHostDatabase())
            {

            }
            if (builder.IsHostDatabase())
            {
                builder.Entity<StoryTicket>(b =>
                {
                    b.ToTable(ImaarConsts.DbTablePrefix + "StoryTickets", ImaarConsts.DbSchema);
                    b.ConfigureByConvention();
                    b.Property(x => x.Description).HasColumnName(nameof(StoryTicket.Description)).IsRequired();
                    b.HasOne<StoryTicketType>().WithMany().IsRequired().HasForeignKey(x => x.StoryTicketTypeId).OnDelete(DeleteBehavior.NoAction);
                    b.HasOne<UserProfile>().WithMany().IsRequired().HasForeignKey(x => x.TicketCreatorId).OnDelete(DeleteBehavior.NoAction);
                    b.HasOne<Story>().WithMany().IsRequired().HasForeignKey(x => x.StoryAgainstId).OnDelete(DeleteBehavior.NoAction);
                });

            }

        }

        if (builder.IsHostDatabase())
        {
            builder.Entity<UserSavedItem>(b =>
            {
                b.ToTable(ImaarConsts.DbTablePrefix + "UserSavedItems", ImaarConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.SourceId).HasColumnName(nameof(UserSavedItem.SourceId)).IsRequired();
                b.Property(x => x.SavedItemType).HasColumnName(nameof(UserSavedItem.SavedItemType));
                b.HasOne<UserProfile>().WithMany().IsRequired().HasForeignKey(x => x.UserProfileId).OnDelete(DeleteBehavior.NoAction);
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<UserProfile>(b =>
            {
                b.ToTable(ImaarConsts.DbTablePrefix + "UserProfiles", ImaarConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.SecurityNumber).HasColumnName(nameof(UserProfile.SecurityNumber)).IsRequired();
                b.Property(x => x.BiologicalSex).HasColumnName(nameof(UserProfile.BiologicalSex));
                b.Property(x => x.DateOfBirth).HasColumnName(nameof(UserProfile.DateOfBirth));
                b.Property(x => x.Latitude).HasColumnName(nameof(UserProfile.Latitude));
                b.Property(x => x.Longitude).HasColumnName(nameof(UserProfile.Longitude));
                b.Property(x => x.ProfilePhoto).HasColumnName(nameof(UserProfile.ProfilePhoto));
                b.Property(x => x.FirstName).HasColumnName(nameof(UserProfile.FirstName)).IsRequired();
                b.Property(x => x.LastName).HasColumnName(nameof(UserProfile.LastName)).IsRequired();
                b.Property(x => x.PhoneNumber).HasColumnName(nameof(UserProfile.PhoneNumber)).IsRequired();
                b.Property(x => x.Email).HasColumnName(nameof(UserProfile.Email)).IsRequired();
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<Advertisement>(b =>
            {
                b.ToTable(ImaarConsts.DbTablePrefix + "Advertisements", ImaarConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Title).HasColumnName(nameof(Advertisement.Title));
                b.Property(x => x.SubTitle).HasColumnName(nameof(Advertisement.SubTitle));
                b.Property(x => x.File).HasColumnName(nameof(Advertisement.File)).IsRequired();
                b.Property(x => x.FromDateTime).HasColumnName(nameof(Advertisement.FromDateTime));
                b.Property(x => x.ToDateTime).HasColumnName(nameof(Advertisement.ToDateTime));
                b.Property(x => x.Order).HasColumnName(nameof(Advertisement.Order));
                b.Property(x => x.IsActive).HasColumnName(nameof(Advertisement.IsActive));
                b.HasOne<UserProfile>().WithMany().HasForeignKey(x => x.UserProfileId).OnDelete(DeleteBehavior.SetNull);
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<VacancyAdditionalFeature>(b =>
            {
                b.ToTable(ImaarConsts.DbTablePrefix + "VacancyAdditionalFeatures", ImaarConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Name).HasColumnName(nameof(VacancyAdditionalFeature.Name)).IsRequired();
                b.Property(x => x.Order).HasColumnName(nameof(VacancyAdditionalFeature.Order));
                b.Property(x => x.IsActive).HasColumnName(nameof(VacancyAdditionalFeature.IsActive));
            });

        }

        if (builder.IsHostDatabase())
        {
            builder.Entity<Vacancy>(b =>
            {
                b.ToTable(ImaarConsts.DbTablePrefix + "Vacancies", ImaarConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Title).HasColumnName(nameof(Vacancy.Title)).IsRequired();
                b.Property(x => x.Description).HasColumnName(nameof(Vacancy.Description)).IsRequired();
                b.Property(x => x.Location).HasColumnName(nameof(Vacancy.Location)).IsRequired();
                b.Property(x => x.Number).HasColumnName(nameof(Vacancy.Number)).IsRequired();
                b.Property(x => x.Latitude).HasColumnName(nameof(Vacancy.Latitude));
                b.Property(x => x.Longitude).HasColumnName(nameof(Vacancy.Longitude));
                b.Property(x => x.DateOfPublish).HasColumnName(nameof(Vacancy.DateOfPublish));
                b.Property(x => x.ExpectedExperience).HasColumnName(nameof(Vacancy.ExpectedExperience));
                b.Property(x => x.EducationLevel).HasColumnName(nameof(Vacancy.EducationLevel));
                b.Property(x => x.WorkSchedule).HasColumnName(nameof(Vacancy.WorkSchedule));
                b.Property(x => x.EmploymentType).HasColumnName(nameof(Vacancy.EmploymentType));
                b.Property(x => x.BiologicalSex).HasColumnName(nameof(Vacancy.BiologicalSex));
                b.Property(x => x.Languages).HasColumnName(nameof(Vacancy.Languages));
                b.Property(x => x.DriveLicense).HasColumnName(nameof(Vacancy.DriveLicense));
                b.Property(x => x.Salary).HasColumnName(nameof(Vacancy.Salary));
                b.Property(x => x.ViewCounter).HasColumnName(nameof(Vacancy.ViewCounter));
                b.Property(x => x.OrderCounter).HasColumnName(nameof(Vacancy.OrderCounter));
                b.HasOne<ServiceType>().WithMany().IsRequired().HasForeignKey(x => x.ServiceTypeId).OnDelete(DeleteBehavior.NoAction);
                b.HasOne<UserProfile>().WithMany().IsRequired().HasForeignKey(x => x.UserProfileId).OnDelete(DeleteBehavior.NoAction);
                b.HasMany(x => x.VacancyAdditionalFeatures).WithOne().HasForeignKey(x => x.VacancyId).IsRequired().OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<VacancyVacancyAdditionalFeature>(b =>
            {
                b.ToTable(ImaarConsts.DbTablePrefix + "VacancyVacancyAdditionalFeature", ImaarConsts.DbSchema);
                b.ConfigureByConvention();

                b.HasKey(
                    x => new { x.VacancyId, x.VacancyAdditionalFeatureId }
                );

                b.HasOne<Vacancy>().WithMany(x => x.VacancyAdditionalFeatures).HasForeignKey(x => x.VacancyId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                b.HasOne<VacancyAdditionalFeature>().WithMany().HasForeignKey(x => x.VacancyAdditionalFeatureId).IsRequired().OnDelete(DeleteBehavior.Cascade);

                b.HasIndex(
                        x => new { x.VacancyId, x.VacancyAdditionalFeatureId }
                );
            });
        }

        if (builder.IsHostDatabase())
        {
            builder.Entity<Building>(b =>
            {
                b.ToTable(ImaarConsts.DbTablePrefix + "Buildings", ImaarConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.MainTitle).HasColumnName(nameof(Building.MainTitle)).IsRequired();
                b.Property(x => x.Description).HasColumnName(nameof(Building.Description)).IsRequired();
                b.Property(x => x.Price).HasColumnName(nameof(Building.Price)).IsRequired();
                b.Property(x => x.BuildingArea).HasColumnName(nameof(Building.BuildingArea)).IsRequired();
                b.Property(x => x.NumberOfRooms).HasColumnName(nameof(Building.NumberOfRooms)).IsRequired();
                b.Property(x => x.NumberOfBaths).HasColumnName(nameof(Building.NumberOfBaths)).IsRequired();
                b.Property(x => x.FloorNo).HasColumnName(nameof(Building.FloorNo)).IsRequired();
                b.Property(x => x.Latitude).HasColumnName(nameof(Building.Latitude));
                b.Property(x => x.Longitude).HasColumnName(nameof(Building.Longitude));
                b.Property(x => x.ViewCounter).HasColumnName(nameof(Building.ViewCounter));
                b.Property(x => x.OrderCounter).HasColumnName(nameof(Building.OrderCounter));
                b.HasOne<Region>().WithMany().IsRequired().HasForeignKey(x => x.RegionId).OnDelete(DeleteBehavior.NoAction);
                b.HasOne<FurnishingLevel>().WithMany().IsRequired().HasForeignKey(x => x.FurnishingLevelId).OnDelete(DeleteBehavior.NoAction);
                b.HasOne<BuildingFacade>().WithMany().IsRequired().HasForeignKey(x => x.BuildingFacadeId).OnDelete(DeleteBehavior.NoAction);
                b.HasOne<ServiceType>().WithMany().IsRequired().HasForeignKey(x => x.ServiceTypeId).OnDelete(DeleteBehavior.NoAction);
                b.HasOne<UserProfile>().WithMany().IsRequired().HasForeignKey(x => x.UserProfileId).OnDelete(DeleteBehavior.NoAction);
                b.HasMany(x => x.MainAmenities).WithOne().HasForeignKey(x => x.BuildingId).IsRequired().OnDelete(DeleteBehavior.NoAction);
                b.HasMany(x => x.SecondaryAmenities).WithOne().HasForeignKey(x => x.BuildingId).IsRequired().OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<BuildingMainAmenity>(b =>
            {
                b.ToTable(ImaarConsts.DbTablePrefix + "BuildingMainAmenity", ImaarConsts.DbSchema);
                b.ConfigureByConvention();

                b.HasKey(
                    x => new { x.BuildingId, x.MainAmenityId }
                );

                b.HasOne<Building>().WithMany(x => x.MainAmenities).HasForeignKey(x => x.BuildingId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                b.HasOne<MainAmenity>().WithMany().HasForeignKey(x => x.MainAmenityId).IsRequired().OnDelete(DeleteBehavior.Cascade);

                b.HasIndex(
                        x => new { x.BuildingId, x.MainAmenityId }
                );
            });
            builder.Entity<BuildingSecondaryAmenity>(b =>
            {
                b.ToTable(ImaarConsts.DbTablePrefix + "BuildingSecondaryAmenity", ImaarConsts.DbSchema);
                b.ConfigureByConvention();

                b.HasKey(
                    x => new { x.BuildingId, x.SecondaryAmenityId }
                );

                b.HasOne<Building>().WithMany(x => x.SecondaryAmenities).HasForeignKey(x => x.BuildingId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                b.HasOne<SecondaryAmenity>().WithMany().HasForeignKey(x => x.SecondaryAmenityId).IsRequired().OnDelete(DeleteBehavior.Cascade);

                b.HasIndex(
                        x => new { x.BuildingId, x.SecondaryAmenityId }
                );
            });
        }

    }
}
