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
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ImaarResource>(name);
    }
}
