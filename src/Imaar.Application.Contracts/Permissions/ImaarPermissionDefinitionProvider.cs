using Imaar.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Imaar.Permissions;

public class ImaarPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(ImaarPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(ImaarPermissions.MyPermission1, L("Permission:MyPermission1"));


        var categoryPermission = myGroup.AddPermission(ImaarPermissions.Categories.Default, L("Permission:Categories"));
        categoryPermission.AddChild(ImaarPermissions.Categories.Create, L("Permission:Create"));
        categoryPermission.AddChild(ImaarPermissions.Categories.Edit, L("Permission:Edit"));
        categoryPermission.AddChild(ImaarPermissions.Categories.Delete, L("Permission:Delete"));

        var userProfilePermission = myGroup.AddPermission(ImaarPermissions.UserProfiles.Default, L("Permission:UserProfiles"));
        userProfilePermission.AddChild(ImaarPermissions.UserProfiles.Create, L("Permission:Create"));
        userProfilePermission.AddChild(ImaarPermissions.UserProfiles.Edit, L("Permission:Edit"));
        userProfilePermission.AddChild(ImaarPermissions.UserProfiles.Delete, L("Permission:Delete"));


        var serviceTypePermission = myGroup.AddPermission(ImaarPermissions.ServiceTypes.Default, L("Permission:ServiceTypes"));
        serviceTypePermission.AddChild(ImaarPermissions.ServiceTypes.Create, L("Permission:Create"));
        serviceTypePermission.AddChild(ImaarPermissions.ServiceTypes.Edit, L("Permission:Edit"));
        serviceTypePermission.AddChild(ImaarPermissions.ServiceTypes.Delete, L("Permission:Delete"));

        var imaarServicePermission = myGroup.AddPermission(ImaarPermissions.ImaarServices.Default, L("Permission:ImaarServices"));
        imaarServicePermission.AddChild(ImaarPermissions.ImaarServices.Create, L("Permission:Create"));
        imaarServicePermission.AddChild(ImaarPermissions.ImaarServices.Edit, L("Permission:Edit"));
        imaarServicePermission.AddChild(ImaarPermissions.ImaarServices.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ImaarResource>(name);
    }
}
