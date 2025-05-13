using Imaar.Categories;
using Imaar.UserProfiles;
using Imaar.ServiceTypes;
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
using Imaar.ImaarServices;
using Imaar.VerificationCodes;
using Imaar.TicketTypes;
using Imaar.Tickets;
using Imaar.Medias;
using Imaar.Stories;
using Imaar.StoryLovers;
using Imaar.Vacancies;

namespace Imaar.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class ImaarDbContext :
    AbpDbContext<ImaarDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
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
            });

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
                b.HasOne<ServiceType>().WithMany().IsRequired().HasForeignKey(x => x.ServiceTypeId).OnDelete(DeleteBehavior.NoAction);
                b.HasOne<UserProfile>().WithMany().IsRequired().HasForeignKey(x => x.UserProfileId).OnDelete(DeleteBehavior.NoAction);
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
                b.HasOne<ImaarService>().WithMany().HasForeignKey(x => x.ImaarServiceId).OnDelete(DeleteBehavior.SetNull);
                b.HasOne<Vacancy>().WithMany().HasForeignKey(x => x.VacancyId).OnDelete(DeleteBehavior.SetNull);
                b.HasOne<Story>().WithMany().HasForeignKey(x => x.StoryId).OnDelete(DeleteBehavior.SetNull);
            });

        }
    }
}
