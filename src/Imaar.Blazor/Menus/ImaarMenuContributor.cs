using System.Threading.Tasks;
using Imaar.Localization;
using Imaar.MultiTenancy;
using Imaar.Permissions;
using Volo.Abp.Identity.Blazor;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.TenantManagement.Blazor.Navigation;
using Volo.Abp.UI.Navigation;

namespace Imaar.Blazor.Menus;

public class ImaarMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var administration = context.Menu.GetAdministration();
        var l = context.GetLocalizer<ImaarResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                ImaarMenus.Home,
                l["Menu:Home"],
                "/",
                icon: "fas fa-home",
                order: 0
            )
        );

        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 3);

        context.Menu.AddItem(
    new ApplicationMenuItem(
        ImaarMenus.Categories,
        l["Menu:Categories"],
        url: "/categories",
        icon: "fa fa-file-alt",
        requiredPermissionName: ImaarPermissions.Categories.Default)
        );



        context.Menu.AddItem(
            new ApplicationMenuItem(
                ImaarMenus.VerificationCodes,
                l["Menu:VerificationCodes"],
                url: "/verification-codes",
                icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.VerificationCodes.Default)
        );

        context.Menu.AddItem(
                new ApplicationMenuItem("UserProfiles", l["Menu:UserProfiles"], icon: "fa fa-file-alt")
          .AddItem(new ApplicationMenuItem(
                ImaarMenus.UserProfiles,
                l["Menu:UserProfiles"],
                url: "/user-profiles",
                icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.UserProfiles.Default))
          .AddItem(new ApplicationMenuItem(
                ImaarMenus.UserEvalauations,
                l["Menu:UserEvalauations"],
                url: "/userEvalauations",
                icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.UserEvalauations.Default))
          .AddItem(new ApplicationMenuItem(
                ImaarMenus.UserWorksExhibitions,
                l["Menu:UserWorksExhibitions"],
                url: "/user-works-exhibitions",
                icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.UserWorksExhibitions.Default))
          .AddItem(new ApplicationMenuItem(
                ImaarMenus.UserFollows,
                l["Menu:UserFollows"],
                url: "/user-follows",
                icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.UserFollows.Default))
          .AddItem(new ApplicationMenuItem(
                ImaarMenus.TicketTypes,
                l["Menu:TicketTypes"],
                url: "/ticket-types",
                icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.TicketTypes.Default))
          .AddItem(new ApplicationMenuItem(
                ImaarMenus.Tickets,
                l["Menu:Tickets"],
                url: "/tickets",
                icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.Tickets.Default))
                    );
        context.Menu.AddItem(
                  new ApplicationMenuItem("ImaarServices", l["Menu:ImaarServices"], icon: "fa fa-file-alt")
            .AddItem(new ApplicationMenuItem(
                ImaarMenus.ImaarServices,
                l["Menu:ImaarServices"],
                url: "/imaar-services",
                icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.ImaarServices.Default))
            .AddItem(new ApplicationMenuItem(
                ImaarMenus.ServiceEvaluations,
                l["Menu:ServiceEvaluations"],
                url: "/service-evaluations",
icon:           "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.ServiceEvaluations.Default))
            .AddItem(new ApplicationMenuItem(
                ImaarMenus.ServiceTicketTypes,
                l["Menu:ServiceTicketTypes"],
                url: "/service-ticket-types",
                icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.ServiceTicketTypes.Default))
            .AddItem(new ApplicationMenuItem(
                ImaarMenus.ServiceTickets,
                l["Menu:ServiceTickets"],
                url: "/service-tickets",
                icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.ServiceTickets.Default))
            );
        context.Menu.AddItem(
                   new ApplicationMenuItem("Stories", l["Menu:Stories"], icon: "fa fa-file-alt")
                   .AddItem(new ApplicationMenuItem(
                            ImaarMenus.Stories,
                            l["Menu:Stories"],
                            url: "/stories",
                            icon: "fa fa-file-alt",
                            requiredPermissionName: ImaarPermissions.Stories.Default))
                   .AddItem(new ApplicationMenuItem(
                            ImaarMenus.StoryLovers,
                            l["Menu:StoryLovers"],
                            url: "/story-lovers",
                            icon: "fa fa-file-alt",
                            requiredPermissionName: ImaarPermissions.StoryLovers.Default))
                   .AddItem(new ApplicationMenuItem(
                            ImaarMenus.StoryTicketTypes,
                            l["Menu:StoryTicketTypes"],
                            url: "/story-ticket-types",
                            icon: "fa fa-file-alt",
                            requiredPermissionName: ImaarPermissions.StoryTicketTypes.Default))
                   .AddItem(new ApplicationMenuItem(
                            ImaarMenus.StoryTickets,
                            l["Menu:StoryTickets"],
                            url: "/story-tickets",
                            icon: "fa fa-file-alt",
                            requiredPermissionName: ImaarPermissions.StoryTickets.Default))

                   );
        context.Menu.AddItem(
                new ApplicationMenuItem("Buildings", l["Menu:Buildings"], icon: "fa fa-file-alt")
          .AddItem(new ApplicationMenuItem(
                ImaarMenus.Buildings,
                l["Menu:Buildings"],
                url: "/buildings",
                icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.Buildings.Default))
          .AddItem(new ApplicationMenuItem(
                ImaarMenus.BuildingFacades,
                l["Menu:BuildingFacades"],
                url: "/building-facades",
                icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.BuildingFacades.Default))
          .AddItem(new ApplicationMenuItem(
                ImaarMenus.MainAmenities,
                l["Menu:MainAmenities"],
                url: "/main-amenities",
                icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.MainAmenities.Default))
          .AddItem(new ApplicationMenuItem(
                ImaarMenus.SecondaryAmenities,
                l["Menu:SecondaryAmenities"],
                url: "/secondary-amenities",
                icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.SecondaryAmenities.Default))
          .AddItem(new ApplicationMenuItem(
                ImaarMenus.FurnishingLevels,
                l["Menu:FurnishingLevels"],
                url: "/furnishing-levels",
                icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.FurnishingLevels.Default))

                    );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                ImaarMenus.Vacancies,
                l["Menu:Vacancies"],
                url: "/vacancies",
                icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.Vacancies.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                ImaarMenus.Medias,
                l["Menu:Medias"],
                url: "/medias",
                icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.Medias.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                ImaarMenus.Cities,
                l["Menu:Cities"],
                url: "/cities",
icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.Cities.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                ImaarMenus.Regions,
                l["Menu:Regions"],
                url: "/regions",
icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.Regions.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                ImaarMenus.NotificationTypes,
                l["Menu:NotificationTypes"],
                url: "/notification-types",
                icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.NotificationTypes.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                ImaarMenus.Notifications,
                l["Menu:Notifications"],
                url: "/notifications",
                icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.Notifications.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                ImaarMenus.UserSavedItems,
                l["Menu:UserSavedItems"],
                url: "/user-saved-items",
                icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.UserSavedItems.Default)
        );
        return Task.CompletedTask;
    }
}
