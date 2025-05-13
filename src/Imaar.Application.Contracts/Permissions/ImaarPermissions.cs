namespace Imaar.Permissions;

public static class ImaarPermissions
{
    public const string GroupName = "Imaar";

    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";

    public static class Categories
    {
        public const string Default = GroupName + ".Categories";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class UserProfiles
    {
        public const string Default = GroupName + ".UserProfiles";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class ServiceTypes
    {
        public const string Default = GroupName + ".ServiceTypes";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class ImaarServices
    {
        public const string Default = GroupName + ".ImaarServices";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
    public static class VerificationCodes
    {
        public const string Default = GroupName + ".VerificationCodes";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class TicketTypes
    {
        public const string Default = GroupName + ".TicketTypes";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class Tickets
    {
        public const string Default = GroupName + ".Tickets";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class Stories
    {
        public const string Default = GroupName + ".Stories";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class StoryLovers
    {
        public const string Default = GroupName + ".StoryLovers";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class Vacancies
    {
        public const string Default = GroupName + ".Vacancies";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class Medias
    {
        public const string Default = GroupName + ".Medias";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
}
