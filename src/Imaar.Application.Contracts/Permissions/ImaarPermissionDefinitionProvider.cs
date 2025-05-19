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

        var verificationCodePermission = myGroup.AddPermission(ImaarPermissions.VerificationCodes.Default, L("Permission:VerificationCodes"));
        verificationCodePermission.AddChild(ImaarPermissions.VerificationCodes.Create, L("Permission:Create"));
        verificationCodePermission.AddChild(ImaarPermissions.VerificationCodes.Edit, L("Permission:Edit"));
        verificationCodePermission.AddChild(ImaarPermissions.VerificationCodes.Delete, L("Permission:Delete"));

        var ticketTypePermission = myGroup.AddPermission(ImaarPermissions.TicketTypes.Default, L("Permission:TicketTypes"));
        ticketTypePermission.AddChild(ImaarPermissions.TicketTypes.Create, L("Permission:Create"));
        ticketTypePermission.AddChild(ImaarPermissions.TicketTypes.Edit, L("Permission:Edit"));
        ticketTypePermission.AddChild(ImaarPermissions.TicketTypes.Delete, L("Permission:Delete"));

        var ticketPermission = myGroup.AddPermission(ImaarPermissions.Tickets.Default, L("Permission:Tickets"));
        ticketPermission.AddChild(ImaarPermissions.Tickets.Create, L("Permission:Create"));
        ticketPermission.AddChild(ImaarPermissions.Tickets.Edit, L("Permission:Edit"));
        ticketPermission.AddChild(ImaarPermissions.Tickets.Delete, L("Permission:Delete"));


        var storyPermission = myGroup.AddPermission(ImaarPermissions.Stories.Default, L("Permission:Stories"));
        storyPermission.AddChild(ImaarPermissions.Stories.Create, L("Permission:Create"));
        storyPermission.AddChild(ImaarPermissions.Stories.Edit, L("Permission:Edit"));
        storyPermission.AddChild(ImaarPermissions.Stories.Delete, L("Permission:Delete"));

        var storyLoverPermission = myGroup.AddPermission(ImaarPermissions.StoryLovers.Default, L("Permission:StoryLovers"));
        storyLoverPermission.AddChild(ImaarPermissions.StoryLovers.Create, L("Permission:Create"));
        storyLoverPermission.AddChild(ImaarPermissions.StoryLovers.Edit, L("Permission:Edit"));
        storyLoverPermission.AddChild(ImaarPermissions.StoryLovers.Delete, L("Permission:Delete"));

        var vacancyPermission = myGroup.AddPermission(ImaarPermissions.Vacancies.Default, L("Permission:Vacancies"));
        vacancyPermission.AddChild(ImaarPermissions.Vacancies.Create, L("Permission:Create"));
        vacancyPermission.AddChild(ImaarPermissions.Vacancies.Edit, L("Permission:Edit"));
        vacancyPermission.AddChild(ImaarPermissions.Vacancies.Delete, L("Permission:Delete"));

        var mediaPermission = myGroup.AddPermission(ImaarPermissions.Medias.Default, L("Permission:Medias"));
        mediaPermission.AddChild(ImaarPermissions.Medias.Create, L("Permission:Create"));
        mediaPermission.AddChild(ImaarPermissions.Medias.Edit, L("Permission:Edit"));
        mediaPermission.AddChild(ImaarPermissions.Medias.Delete, L("Permission:Delete"));


        var evalauationPermission = myGroup.AddPermission(ImaarPermissions.Evalauations.Default, L("Permission:Evalauations"));
        evalauationPermission.AddChild(ImaarPermissions.Evalauations.Create, L("Permission:Create"));
        evalauationPermission.AddChild(ImaarPermissions.Evalauations.Edit, L("Permission:Edit"));
        evalauationPermission.AddChild(ImaarPermissions.Evalauations.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ImaarResource>(name);
    }
}
