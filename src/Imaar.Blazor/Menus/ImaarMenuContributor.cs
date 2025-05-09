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
                ImaarMenus.UserProfiles,
                l["Menu:UserProfiles"],
                url: "/user-profiles",
icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.UserProfiles.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                ImaarMenus.ImaarServices,
                l["Menu:ImaarServices"],
                url: "/imaar-services",
                icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.ImaarServices.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                ImaarMenus.VerificationCodes,
                l["Menu:VerificationCodes"],
                url: "/verification-codes",
                icon: "fa fa-file-alt",
                requiredPermissionName: ImaarPermissions.VerificationCodes.Default)
        );
        return Task.CompletedTask;
    }
}
