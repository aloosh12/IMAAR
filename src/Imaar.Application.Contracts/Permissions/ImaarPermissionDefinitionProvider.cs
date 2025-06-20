﻿using Imaar.Localization;
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


        var userEvalauationPermission = myGroup.AddPermission(ImaarPermissions.UserEvalauations.Default, L("Permission:UserEvalauations"));
        userEvalauationPermission.AddChild(ImaarPermissions.UserEvalauations.Create, L("Permission:Create"));
        userEvalauationPermission.AddChild(ImaarPermissions.UserEvalauations.Edit, L("Permission:Edit"));
        userEvalauationPermission.AddChild(ImaarPermissions.UserEvalauations.Delete, L("Permission:Delete"));

        var serviceEvaluationPermission = myGroup.AddPermission(ImaarPermissions.ServiceEvaluations.Default, L("Permission:ServiceEvaluations"));
        serviceEvaluationPermission.AddChild(ImaarPermissions.ServiceEvaluations.Create, L("Permission:Create"));
        serviceEvaluationPermission.AddChild(ImaarPermissions.ServiceEvaluations.Edit, L("Permission:Edit"));
        serviceEvaluationPermission.AddChild(ImaarPermissions.ServiceEvaluations.Delete, L("Permission:Delete"));

        var userWorksExhibitionPermission = myGroup.AddPermission(ImaarPermissions.UserWorksExhibitions.Default, L("Permission:UserWorksExhibitions"));
        userWorksExhibitionPermission.AddChild(ImaarPermissions.UserWorksExhibitions.Create, L("Permission:Create"));
        userWorksExhibitionPermission.AddChild(ImaarPermissions.UserWorksExhibitions.Edit, L("Permission:Edit"));
        userWorksExhibitionPermission.AddChild(ImaarPermissions.UserWorksExhibitions.Delete, L("Permission:Delete"));


        var userFollowPermission = myGroup.AddPermission(ImaarPermissions.UserFollows.Default, L("Permission:UserFollows"));
        userFollowPermission.AddChild(ImaarPermissions.UserFollows.Create, L("Permission:Create"));
        userFollowPermission.AddChild(ImaarPermissions.UserFollows.Edit, L("Permission:Edit"));
        userFollowPermission.AddChild(ImaarPermissions.UserFollows.Delete, L("Permission:Delete"));


        var cityPermission = myGroup.AddPermission(ImaarPermissions.Cities.Default, L("Permission:Cities"));
        cityPermission.AddChild(ImaarPermissions.Cities.Create, L("Permission:Create"));
        cityPermission.AddChild(ImaarPermissions.Cities.Edit, L("Permission:Edit"));
        cityPermission.AddChild(ImaarPermissions.Cities.Delete, L("Permission:Delete"));

        var regionPermission = myGroup.AddPermission(ImaarPermissions.Regions.Default, L("Permission:Regions"));
        regionPermission.AddChild(ImaarPermissions.Regions.Create, L("Permission:Create"));
        regionPermission.AddChild(ImaarPermissions.Regions.Edit, L("Permission:Edit"));
        regionPermission.AddChild(ImaarPermissions.Regions.Delete, L("Permission:Delete"));

        var furnishingLevelPermission = myGroup.AddPermission(ImaarPermissions.FurnishingLevels.Default, L("Permission:FurnishingLevels"));
        furnishingLevelPermission.AddChild(ImaarPermissions.FurnishingLevels.Create, L("Permission:Create"));
        furnishingLevelPermission.AddChild(ImaarPermissions.FurnishingLevels.Edit, L("Permission:Edit"));
        furnishingLevelPermission.AddChild(ImaarPermissions.FurnishingLevels.Delete, L("Permission:Delete"));

        var buildingFacadePermission = myGroup.AddPermission(ImaarPermissions.BuildingFacades.Default, L("Permission:BuildingFacades"));
        buildingFacadePermission.AddChild(ImaarPermissions.BuildingFacades.Create, L("Permission:Create"));
        buildingFacadePermission.AddChild(ImaarPermissions.BuildingFacades.Edit, L("Permission:Edit"));
        buildingFacadePermission.AddChild(ImaarPermissions.BuildingFacades.Delete, L("Permission:Delete"));

        var mainAmenityPermission = myGroup.AddPermission(ImaarPermissions.MainAmenities.Default, L("Permission:MainAmenities"));
        mainAmenityPermission.AddChild(ImaarPermissions.MainAmenities.Create, L("Permission:Create"));
        mainAmenityPermission.AddChild(ImaarPermissions.MainAmenities.Edit, L("Permission:Edit"));
        mainAmenityPermission.AddChild(ImaarPermissions.MainAmenities.Delete, L("Permission:Delete"));

        var secondaryAmenityPermission = myGroup.AddPermission(ImaarPermissions.SecondaryAmenities.Default, L("Permission:SecondaryAmenities"));
        secondaryAmenityPermission.AddChild(ImaarPermissions.SecondaryAmenities.Create, L("Permission:Create"));
        secondaryAmenityPermission.AddChild(ImaarPermissions.SecondaryAmenities.Edit, L("Permission:Edit"));
        secondaryAmenityPermission.AddChild(ImaarPermissions.SecondaryAmenities.Delete, L("Permission:Delete"));

        var buildingPermission = myGroup.AddPermission(ImaarPermissions.Buildings.Default, L("Permission:Buildings"));
        buildingPermission.AddChild(ImaarPermissions.Buildings.Create, L("Permission:Create"));
        buildingPermission.AddChild(ImaarPermissions.Buildings.Edit, L("Permission:Edit"));
        buildingPermission.AddChild(ImaarPermissions.Buildings.Delete, L("Permission:Delete"));


        var serviceTicketTypePermission = myGroup.AddPermission(ImaarPermissions.ServiceTicketTypes.Default, L("Permission:ServiceTicketTypes"));
        serviceTicketTypePermission.AddChild(ImaarPermissions.ServiceTicketTypes.Create, L("Permission:Create"));
        serviceTicketTypePermission.AddChild(ImaarPermissions.ServiceTicketTypes.Edit, L("Permission:Edit"));
        serviceTicketTypePermission.AddChild(ImaarPermissions.ServiceTicketTypes.Delete, L("Permission:Delete"));

        var notificationTypePermission = myGroup.AddPermission(ImaarPermissions.NotificationTypes.Default, L("Permission:NotificationTypes"));
        notificationTypePermission.AddChild(ImaarPermissions.NotificationTypes.Create, L("Permission:Create"));
        notificationTypePermission.AddChild(ImaarPermissions.NotificationTypes.Edit, L("Permission:Edit"));
        notificationTypePermission.AddChild(ImaarPermissions.NotificationTypes.Delete, L("Permission:Delete"));

        var notificationPermission = myGroup.AddPermission(ImaarPermissions.Notifications.Default, L("Permission:Notifications"));
        notificationPermission.AddChild(ImaarPermissions.Notifications.Create, L("Permission:Create"));
        notificationPermission.AddChild(ImaarPermissions.Notifications.Edit, L("Permission:Edit"));
        notificationPermission.AddChild(ImaarPermissions.Notifications.Delete, L("Permission:Delete"));

        var serviceTicketPermission = myGroup.AddPermission(ImaarPermissions.ServiceTickets.Default, L("Permission:ServiceTickets"));
        serviceTicketPermission.AddChild(ImaarPermissions.ServiceTickets.Create, L("Permission:Create"));
        serviceTicketPermission.AddChild(ImaarPermissions.ServiceTickets.Edit, L("Permission:Edit"));
        serviceTicketPermission.AddChild(ImaarPermissions.ServiceTickets.Delete, L("Permission:Delete"));


        var storyTicketTypePermission = myGroup.AddPermission(ImaarPermissions.StoryTicketTypes.Default, L("Permission:StoryTicketTypes"));
        storyTicketTypePermission.AddChild(ImaarPermissions.StoryTicketTypes.Create, L("Permission:Create"));
        storyTicketTypePermission.AddChild(ImaarPermissions.StoryTicketTypes.Edit, L("Permission:Edit"));
        storyTicketTypePermission.AddChild(ImaarPermissions.StoryTicketTypes.Delete, L("Permission:Delete"));

        var storyTicketPermission = myGroup.AddPermission(ImaarPermissions.StoryTickets.Default, L("Permission:StoryTickets"));
        storyTicketPermission.AddChild(ImaarPermissions.StoryTickets.Create, L("Permission:Create"));
        storyTicketPermission.AddChild(ImaarPermissions.StoryTickets.Edit, L("Permission:Edit"));
        storyTicketPermission.AddChild(ImaarPermissions.StoryTickets.Delete, L("Permission:Delete"));


        var userSavedItemPermission = myGroup.AddPermission(ImaarPermissions.UserSavedItems.Default, L("Permission:UserSavedItems"));
        userSavedItemPermission.AddChild(ImaarPermissions.UserSavedItems.Create, L("Permission:Create"));
        userSavedItemPermission.AddChild(ImaarPermissions.UserSavedItems.Edit, L("Permission:Edit"));
        userSavedItemPermission.AddChild(ImaarPermissions.UserSavedItems.Delete, L("Permission:Delete"));


        var advertisementPermission = myGroup.AddPermission(ImaarPermissions.Advertisements.Default, L("Permission:Advertisements"));
        advertisementPermission.AddChild(ImaarPermissions.Advertisements.Create, L("Permission:Create"));
        advertisementPermission.AddChild(ImaarPermissions.Advertisements.Edit, L("Permission:Edit"));
        advertisementPermission.AddChild(ImaarPermissions.Advertisements.Delete, L("Permission:Delete"));


        var vacancyAdditionalFeaturePermission = myGroup.AddPermission(ImaarPermissions.VacancyAdditionalFeatures.Default, L("Permission:VacancyAdditionalFeatures"));
        vacancyAdditionalFeaturePermission.AddChild(ImaarPermissions.VacancyAdditionalFeatures.Create, L("Permission:Create"));
        vacancyAdditionalFeaturePermission.AddChild(ImaarPermissions.VacancyAdditionalFeatures.Edit, L("Permission:Edit"));
        vacancyAdditionalFeaturePermission.AddChild(ImaarPermissions.VacancyAdditionalFeatures.Delete, L("Permission:Delete"));


        var buildingEvaluationPermission = myGroup.AddPermission(ImaarPermissions.BuildingEvaluations.Default, L("Permission:BuildingEvaluations"));
        buildingEvaluationPermission.AddChild(ImaarPermissions.BuildingEvaluations.Create, L("Permission:Create"));
        buildingEvaluationPermission.AddChild(ImaarPermissions.BuildingEvaluations.Edit, L("Permission:Edit"));
        buildingEvaluationPermission.AddChild(ImaarPermissions.BuildingEvaluations.Delete, L("Permission:Delete"));

        var vacancyEvaluationPermission = myGroup.AddPermission(ImaarPermissions.VacancyEvaluations.Default, L("Permission:VacancyEvaluations"));
        vacancyEvaluationPermission.AddChild(ImaarPermissions.VacancyEvaluations.Create, L("Permission:Create"));
        vacancyEvaluationPermission.AddChild(ImaarPermissions.VacancyEvaluations.Edit, L("Permission:Edit"));
        vacancyEvaluationPermission.AddChild(ImaarPermissions.VacancyEvaluations.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ImaarResource>(name);
    }
}
